using Newtonsoft.Json;

namespace Uber.Services.Osm.Models
{
    public class Route
    {
        [JsonProperty("legs")]
        public List<Leg> Legs { get; set; }

        [JsonProperty("weight_name")]
        public string WeightName { get; set; }

        [JsonProperty("weight")]
        public double Weight { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }
        public override string ToString()
        {
            return $"Distance: {Distance} meters, Duration: {Duration} seconds";
        }
    }
}
