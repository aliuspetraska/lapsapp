using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LapsMobileApp.Models
{
    public class Track
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("coordinates")]
        public string Coordinates { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
