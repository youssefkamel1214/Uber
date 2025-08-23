using Newtonsoft.Json;

namespace Uber.Services.Osm.Models
{
    public class Waypoint
    {
        [JsonProperty("hint")]
        public string Hint { get; set; }

        [JsonProperty("location")]
        public List<double> Location { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }
    }
}
