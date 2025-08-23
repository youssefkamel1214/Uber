using Newtonsoft.Json;
using Uber.Services.Interfaces;
using Uber.Services.Osm.Models;

namespace Uber.Services.Osm
{
    public class OsmRouteDistanceService : IRouteDistanceService
    {
        private readonly HttpClient _httpClient;

        public OsmRouteDistanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetDistanceAsync(double startLatitude, double startLongitude, double endLatitude, double endLongitude)
        {
            var response = await _httpClient.GetAsync(
               $"route/v1/driving/{startLongitude},{startLatitude};{endLongitude},{endLatitude}?overview=false");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<OsmResponse>(content);
            return (decimal)result.Routes[0].Distance / 1000M; // Convert meters to kilometers
        }
    }
}
