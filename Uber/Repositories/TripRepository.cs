
using Microsoft.EntityFrameworkCore;
using Uber.Data;
using Uber.Models.Domain;
using Uber.Models.Responses;
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

        public async Task<List<TripDataResult>> getavailbleTripsAsync(string UID)
        {
            var list=await _db.trips.AsNoTracking().Include(trip=>trip.Passenger).Where(trip=>trip.Status==TripStatue.DriverWaiting
            &&!_db.cancellations.Any(c=>c.TripId==trip.TripId&&c.CancelledBy==UID)).Select(tr=>new TripDataResult()
            {
                BasePrice=tr.BasePrice,
                Distance=tr.Distance,
                destination=tr.Destination,
                source=tr.Source,
                PassengerName=tr.Passenger.UserName,
                PassengerRating=tr.Passenger.rating,
                RequestTime=tr.RequestTime,
                Status=tr.Status,
                tripId=tr.TripId.ToString(),
                

            }).ToListAsync();
            return list;
        }

        public async Task<List<Trip>> getRestOfTripsThathasTenders(Tender tender)
        {
            var list = await _db.trips.AsNoTracking().Where(tr=>
            _db.tenders.Any(tend=>tend.TenderId!=tender.TenderId&&tend.DriverId==tender.DriverId)).ToListAsync();
            return list;
        }

        public async Task<Trip?> getTripByIdAsync(Guid tripId)
        {
            Trip? trip = await _db.trips.FirstOrDefaultAsync(tr=>tr.TripId==tripId);
            return trip;
        }

        public async Task updateRestOfTripsThathasTenders(Tender tender)
        {
            await _db.trips.Where(tr => tr.Status == TripStatue.WaitingForConifirmationOnTender &&
            _db.tenders.Any(tend => tend.TenderId != tender.TenderId && tend.DriverId == tender.DriverId)).
            ExecuteUpdateAsync(s=>s.SetProperty(trip=>trip.Status,TripStatue.DriverWaiting));
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
            existingTrip.EndTime = trip.EndTime;
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
