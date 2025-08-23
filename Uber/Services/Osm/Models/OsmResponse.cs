using Newtonsoft.Json;

namespace Uber.Services.Osm.Models
{
    public class OsmResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("routes")]
        public List<Route> Routes { get; set; }

        [JsonProperty("waypoints")]
        public List<Waypoint> Waypoints { get; set; }
    }
}
