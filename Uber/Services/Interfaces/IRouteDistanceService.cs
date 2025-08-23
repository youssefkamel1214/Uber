namespace Uber.Services.Interfaces
{
    public interface IRouteDistanceService
    {
        public  Task<decimal> GetDistanceAsync(double startLatitude, double startLongitude, double endLatitude, double endLongitude);
    }
}
