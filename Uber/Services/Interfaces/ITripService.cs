using Uber.Models.Domain;
using Uber.Models.Responses;
using Uber.Utils;

namespace Uber.Services.Interfaces
{
    public interface ITripService
    {
        public Task<ApiResponse> createTripRequest(Trip trip);
        public Task<ApiResponse> createTenderOnTrip(Tender tender);
        public Task<ApiResponse> acceptTenderOffer(Guid tenderId, string userId);
        public Task<ApiResponse> rejectTenderOffer(Guid tenderId, string userId);
        public Task<ApiResponse>DriverConfirmTender(Guid tenderId, string userId);
        public Task<ApiResponse> startTrip(Guid tripId,string driverId);
        public Task<ApiResponse> endTrip(Guid tripId, string driverId);
        public Task<ApiResponse> cancelTrip(Guid tripId,string UID);
        public Task<ApiResponse> AddReviewforTrip(Review review);
        public Task<ApiResponse> updateExistedTender(Guid tenderId, string userId, decimal offeredPrice);
    }
}
