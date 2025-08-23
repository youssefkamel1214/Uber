using Uber.Models.Domain;
using Uber.Models.Responses;

namespace Uber.Repositories.Interfaces
{
    public interface ITripRepository
    {
        public Task<Trip> createtripRequest(Trip trip);
        public Task<Trip?> getTripByIdAsync(Guid tripId);
        public Task<List<TripDataResult>>getavailbleTripsAsync(string UID);
        public Task<bool> updatetripAsync(Trip trip);
        public Task<bool> findIFUserhasOpenedTripRequast(string PassengerID);
        public Task<List<Trip>> getRestOfTripsThathasTenders(Tender tender);
        public  Task updateRestOfTripsThathasTenders(Tender tender);
    }
}
