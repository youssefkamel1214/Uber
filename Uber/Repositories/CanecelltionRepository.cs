using Microsoft.EntityFrameworkCore;
using Uber.Data;
using Uber.Models.Domain;
using Uber.Repositories.Interfaces;

namespace Uber.Repositories
{
    public class CanecelltionRepository : ICancellRepository
    {
        public UberAuthDatabase _db;
        public CanecelltionRepository(UberAuthDatabase uberDatabase)
        {
            _db = uberDatabase;
        }
        public async Task<Cancellation> AddCancellationAsync(Cancellation cancellation)
        {
                cancellation.CancellationId = Guid.NewGuid();
                await _db.cancellations.AddAsync(cancellation);
                var result = await _db.SaveChangesAsync();
                if (result > 0)
                {
                    return cancellation;
                }
                else
                    throw new Exception("could not create Cancelltion");

        }

        public async Task<List<Cancellation>> getallCanelltionOfPassengerAsync(string passengerId)
        {
            var cancellations = await ( from canc in _db.cancellations
                                        join tr in _db.trips on canc.TripId equals tr.TripId
                                        where tr.PassengerId == passengerId
                                        select canc).ToListAsync();
            return cancellations;
        }

        public async Task<Cancellation?> GetCancellationByIdAsync(Guid canelletionId)
        {
            var existingCancellation = await _db.cancellations.FirstOrDefaultAsync(c => c.CancellationId == canelletionId);
            return existingCancellation;
        }

        public async Task<Cancellation?> GetCancellationsByTripIdAsync(Guid tripId)
        {
            var existingCancellation = await _db.cancellations.FirstOrDefaultAsync(c => c.TripId == tripId);
            return existingCancellation;
        }

        public async Task<Cancellation?> getCancelletionByDriverIAndTripID(string driverId, Guid tripId)
        {
            var existingCancellation = await _db.cancellations.FirstOrDefaultAsync(c => c.TripId == tripId&&c.CancelledBy==driverId);
            return existingCancellation;
        }

        public async Task<bool> UpdateCancellationAsync(Cancellation cancellation)
        {
            var existingCancellation = await _db.cancellations.FirstOrDefaultAsync(c => c.CancellationId == cancellation.CancellationId);
            if (existingCancellation == null)
            {
                throw new Exception("there no caneclltion for this id.");
            }
            existingCancellation.IsRefunded=cancellation.IsRefunded;
            existingCancellation.Reason = cancellation.Reason;
            _db.cancellations.Update(existingCancellation);
            var result = await _db.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            else
            {
               return false;
            }
        }
    }
}
