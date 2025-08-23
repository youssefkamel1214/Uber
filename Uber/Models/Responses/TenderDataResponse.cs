namespace Uber.Models.Responses
{
    public class TenderDataResponse
    {
        public Guid TenderId { get; set; }
        public string DriverName { get; set; }
        public DateTime ExpiryTime { get; set; }
        public decimal OfferedPrice { get; set; }
        public decimal? DriverRating { get; set; }
        public string licensePlate { get; set; }
        public string DriverPhoneNumber { get; set; }
    }
}
