using Uber.Models.Domain;
using Uber.Models.Responses;
using Uber.Utils;

namespace Uber.Services.Interfaces
{
    public interface ITripService
    {
        public Task<TripServiceResponse> createTripRequest(Trip trip);
        public Task<TripServiceResponse> createTenderOnTrip(Tender tender);
        public Task<TripServiceResponse> acceptTenderOffer(Guid tenderId);
        public Task<TripServiceResponse> rejectTenderOffer(Guid tenderId);
        public Task<TripServiceResponse>DriverConfirmTender(Guid tenderId);
        public Task<TripServiceResponse> startTrip(Guid tripId);
        public Task<TripServiceResponse> endTrip(Guid tripId);
        public Task<TripServiceResponse> cancelTrip(Guid tripId, CancelledBy cancelledBy);
        public Task<TripServiceResponse> AddReviewforTrip(Review review);
        public Task<bool> canDriverLithenToTrips(string driverId);
    }
}
