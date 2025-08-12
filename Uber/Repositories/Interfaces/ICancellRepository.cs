using Uber.Models.Domain;

namespace Uber.Repositories.Interfaces
{
    public interface ICancellRepository
    {
        public Task<Cancellation> AddCancellationAsync(Cancellation cancellation);
        public Task<Cancellation?> GetCancellationByTripIdAndDriverIdAsync(Guid canelletionId);
        public Task<Cancellation?> GetCancellationsByTripIdAsync(Guid tripId);
        public Task<bool> UpdateCancellationAsync(Cancellation cancellation);
        public Task<List<Cancellation>> getallCanelltionOfPassengerAsync(string passengerId);
    }
}
