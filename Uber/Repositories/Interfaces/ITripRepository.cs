using Uber.Models.Domain;

namespace Uber.Repositories.Interfaces
{
    public interface ITripRepository
    {
        public Task<Trip> createtripRequest(Trip trip);
        public Task<Trip?> getTripByIdAsync(Guid tripId);
        public Task<List<Trip>>getavailbleTripsAsync();
        public Task<bool> updatetripAsync(Trip trip);
        public Task<bool> findIFUserhasOpenedTripRequast(string PassengerID);
    }
}
