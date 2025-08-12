namespace Uber.Models.Responses
{
    public class TripServiceResponse
    {
        public string?Message { get; set; }
        public bool success { get; set; }
        public List<string> Error { get; set; }
    }
}
