
using Microsoft.EntityFrameworkCore;
using Uber.Data;
using Uber.Models.Domain;
using Uber.Repositories.Interfaces;
using Uber.Utils;
namespace Uber.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly UberAuthDatabase _db;

        public TripRepository(UberAuthDatabase uberDatabase)
        {
            _db = uberDatabase;
        }

        public async Task<Trip> createtripRequest(Trip trip)
        {
            trip.TripId = Guid.NewGuid();
            trip.RequestTime = DateTime.UtcNow;
            await _db.trips.AddAsync(trip);
            await _db.SaveChangesAsync();
            return trip;

        }

        public async Task<bool> findIFUserhasOpenedTripRequast(string PassengerID)
        {
            var result = await _db.trips.AnyAsync(tr=>tr.PassengerId==PassengerID&&!(tr.Status == TripStatue.TripCompleted || tr.Status == TripStatue.TripCancelled));
            return result;
        }

        public async Task<List<Trip>> getavailbleTripsAsync()
        {
            var list=await _db.trips.Where(trip=>trip.Status==TripStatue.DriverWaiting).ToListAsync();
            return list;
        }

        public async Task<Trip?> getTripByIdAsync(Guid tripId)
        {
            Trip? trip = await _db.trips.FirstOrDefaultAsync(tr=>tr.TripId==tripId);
            return trip;
        }

        public async Task<bool> updatetripAsync(Trip trip)
        {
            var existingTrip = await _db.trips.FirstOrDefaultAsync(t => t.TripId == trip.TripId);
            if (existingTrip == null)
            {
                throw new Exception("Trip not found");
            }
            existingTrip.Status = trip.Status;
            existingTrip.FinalPrice = trip.FinalPrice;
            existingTrip.StartTime = trip.StartTime;
            existingTrip.DriverId = trip.DriverId;
            _db.trips.Update(existingTrip);
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
