using Newtonsoft.Json;

namespace Uber.Services.Osm.Models
{
    public class Leg
    {
        [JsonProperty("steps")]
        public List<object> Steps { get; set; }

        [JsonProperty("weight")]
        public double Weight { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }
    }
}
