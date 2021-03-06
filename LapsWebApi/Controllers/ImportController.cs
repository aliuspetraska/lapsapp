﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using LapsWebApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PolylineEncoder.Net.Models;
using PolylineEncoder.Net.Utility;

namespace LapsWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        private IHostingEnvironment _env;

        private WebClient webClient = new WebClient();

        private readonly LapsDbContext _lapsDbContext;

        public ImportController(IHostingEnvironment env, LapsDbContext lapsDbContext = null)
        {
            _env = env;

            _lapsDbContext = lapsDbContext;
        }

        [HttpGet]
        public JsonResult Get()
        {
            if (_lapsDbContext != null)
            {
                try
                {
                    string[] fileEntries = Directory.GetFiles(Path.Combine(_env.WebRootPath, "tracks"));

                    string[] ignoredFiles = {
                        Path.Combine(_env.WebRootPath, "tracks", ".DS_Store"),
                        Path.Combine(_env.WebRootPath, "tracks", ".gitkeep")
                    };

                    fileEntries = fileEntries.Except(ignoredFiles).ToArray();

                    var tracks = _lapsDbContext.Tracks.ToList();

                    foreach (string file in fileEntries)
                    {
                        var trackId = Path.GetFileName(file).Replace(".gpx", string.Empty);

                        var result = CleanupDuplicates(ParseGpxToCoordinates(file));

                        var distance = CalculateTotalDistance(result);

                        var minime = CleanupDuplicates(ReducedAmountOfCoordinates(result));

                        var polyline = BuildEncodedPolyline(minime);

                        // https://www.mapbox.com/api-documentation/#retrieve-a-static-map-from-a-style

                        string url1 = "https://api.mapbox.com/styles/v1/mapbox/streets-v10/static/path-5+f44-0.5(" +
                            HttpUtility.UrlEncode(polyline) + ")/auto/500x300?access_token=pk.eyJ1IjoiYWxpdXNwZXRyYXNrYSIsImEiOiJjajlxd3pmbjg2OGR6MnFxdDk5M205dmI1In0.6zIodwQbHVLbPfbhBEdRhg";

                        webClient.DownloadFile(url1, Path.Combine(_env.WebRootPath, "images", Path.GetFileName(file).Replace(".gpx", string.Empty) + ".png"));

                        if (tracks.Any(row => row.Id == trackId))
                        {
                            // Update Track Image

                            string url2 = "https://api.mapbox.com/styles/v1/mapbox/streets-v10/static/path-5+f44-0.5(" +
                                HttpUtility.UrlEncode(polyline) + ")/auto/500x300?access_token=pk.eyJ1IjoiYWxpdXNwZXRyYXNrYSIsImEiOiJjajlxd3pmbjg2OGR6MnFxdDk5M205dmI1In0.6zIodwQbHVLbPfbhBEdRhg";

                            webClient.DownloadFile(url2, Path.Combine(_env.WebRootPath, "images", Path.GetFileName(file).Replace(".gpx", string.Empty) + ".png"));
                        }
                        else
                        {
                            var track = new Track
                            {
                                Id = trackId,
                                Coordinates = JsonConvert.SerializeObject(result),
                                Distance = distance,
                                Thumbnail = "https://lapsapp.mybluemix.net/images/" + trackId + ".png",
                                Timestamp = DateTime.Now
                            };

                            _lapsDbContext.Add(track);
                            _lapsDbContext.SaveChanges();
                        }
                    }

                    return Json(new
                    {
                        Status = "ok",
                        Message = fileEntries.Count() + " tracks imported."
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        Status = "error",
                        Message = ex.InnerException
                    });
                }
            }
            else
            {
                return Json(new
                {
                   Status = "error",
                   Message = "Can't connect to DB."
                });
            }
        }

        private string BuildEncodedPolyline(List<List<double>> coordinates)
        {
            var utility = new PolylineUtility();

            var geoPoints = new List<IGeoCoordinate>();

            foreach (var point in coordinates)
            {
                geoPoints.Add(new GeoCoordinate(
                    Math.Round(point[1], 3),
                    Math.Round(point[0], 3)
                ));
            }

            return utility.Encode(geoPoints);
        }

        private List<List<double>> ReducedAmountOfCoordinates(List<List<double>> coordinates)
        {
            var output = new List<List<double>>();

            const double pointLimit = 255.0;

            var ratio = pointLimit / coordinates.Count;

            if (ratio > 1.0)
            {
                ratio = 1.0;
            }

            var track = 0.0;
            var last = -1.0;

            foreach (var point in coordinates)
            {
                track += ratio;

                if (Math.Floor(track) > last)
                {
                    output.Add(point);
                    last = Math.Floor(track);
                }
            }

            return output;
        }

        private List<List<double>> ParseGpxToCoordinates(string fileName)
        {
            var output = new List<List<double>>();

            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            XmlNamespaceManager namespaces = new XmlNamespaceManager(document.NameTable);
            namespaces.AddNamespace("gpx", "http://www.topografix.com/GPX/1/1");

            XmlNodeList nodes = document.SelectNodes("//gpx:trkpt", namespaces);

            foreach (XmlNode node in nodes)
            {
                output.Add(new List<double> {
                    Convert.ToDouble(node.Attributes["lon"].Value),
                    Convert.ToDouble(node.Attributes["lat"].Value)
                });
            }

            return output;
        }

        private List<List<double>> CleanupDuplicates(List<List<double>> coordinates)
        {
            var cleaned = new List<List<double>> {
                coordinates[0]
            };

            for (var i = 1; i < coordinates.Count - 1; i++)
            {
                if (Math.Abs(coordinates[i][1] - coordinates[i - 1][1]) > float.MinValue && Math.Abs(coordinates[i][0] - coordinates[i - 1][0]) > float.MinValue)
                {
                    cleaned.Add(coordinates[i]);
                }
            }

            return cleaned;
        }

        private double CalculateTotalDistance(List<List<double>> coordinates)
        {
            var totalKm = 0.0;

            for (var i = 1; i < coordinates.Count; i++)
            {
                totalKm = totalKm + DistanceInKm(coordinates[i - 1][1], coordinates[i - 1][0], coordinates[i][1], coordinates[i][0]);
            }

            return totalKm;
        }

        private double DistanceInKm(double lat1, double lon1, double lat2, double lon2)
        {
            var earthRadiusKm = 6371;

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            lat1 = DegreesToRadians(lat1);
            lat2 = DegreesToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadiusKm * c;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
