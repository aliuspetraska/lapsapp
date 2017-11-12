using System;
using System.Collections.Generic;
using LapsMobileApp.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace LapsMobileApp.Pages
{
    public partial class TrackPage : ContentPage
    {
        private Track _track;

        public TrackPage(Track track)
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "Back");

            _track = track;

            var tick = 1;

            var coordinates = JsonConvert.DeserializeObject<List<List<string>>>(_track.Coordinates);

            webView.Navigated += (sender, e) => 
            {
                webView.IsVisible = true;
                activityIndicator.IsVisible = false;

                Device.StartTimer(TimeSpan.FromMilliseconds(100), () => {
                    var bearing = DegreeBearing(
                        Convert.ToDouble(coordinates[tick - 1][0]),
                        Convert.ToDouble(coordinates[tick - 1][1]),
                        Convert.ToDouble(coordinates[tick][0]),
                        Convert.ToDouble(coordinates[tick][1])
                    );

                    bearing = bearing * -1;

                    Console.WriteLine(bearing);

                    webView.Eval("map.easeTo({ center: [" + coordinates[tick][0] + ", " + coordinates[tick][1] + "], bearing: " + bearing + " });");

                    tick += 1;

                    return true;
                });
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            webView.IsVisible = false;
            activityIndicator.IsVisible = true;
        }

        private double DegreeBearing(double lat1, double lon1, double lat2, double lon2)
        {
            var dLon = DegreesToRadians(lon2 - lon1);
            var dPhi = Math.Log(Math.Tan(DegreesToRadians(lat2) / 2 + Math.PI / 4) / Math.Tan(DegreesToRadians(lat1) / 2 + Math.PI / 4));

            if (Math.Abs(dLon) > Math.PI)
            {
                dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
            }

            return RadiansToBearing(Math.Atan2(dLon, dPhi));
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        private double RadiansToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        private double RadiansToBearing(double radians)
        {
            return (RadiansToDegrees(radians) + 360) % 360;
        }
    }
}
