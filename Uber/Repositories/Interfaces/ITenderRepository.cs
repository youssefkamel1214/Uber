using Uber.Models.Domain;
using Uber.Models.Responses;

namespace Uber.Repositories.Interfaces
{
    public interface ITenderRepository
    {
        public Task<Tender> AddTender(Tender tender);
        public Task<Tender?> GetTenderByIdAsync(Guid tenderId);
        public Task<bool> isThereActiveDriverTenderForThisTrip(Guid tripId, string driverId);
        public Task<bool> updateTender(Tender tender);
        public Task<List<TenderDataResponse>> GetTendersByTripIdAsync(Guid tripId);
        protected Task makeTenderExpire(Tender tender);
        protected Task timeOutTenderConfirm(Tender tender);
        public Task updateRestOfActiveTenders(Tender tender);
        public Task<List<Tender>>  getRestOfTripsThathasTenders(Tender tender);
    }
}
