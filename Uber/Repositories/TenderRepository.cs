using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Uber.Data;
using Uber.Models.Domain;
using Uber.Models.Responses;
using Uber.Repositories.Interfaces;
using Uber.Utils;

namespace Uber.Repositories
{
    public class TenderRepository : ITenderRepository
    {
        private readonly UberAuthDatabase _db;

        public TenderRepository(UberAuthDatabase db)
        {
            _db = db;
        }

        public async Task<Tender> AddTender(Tender tender)
        {
            tender.TenderId = Guid.NewGuid();
            await _db.tenders.AddAsync(tender);
            var result = await _db.SaveChangesAsync();
            if (result > 0)
            {
             return tender;
            }
            else
            {
             throw new Exception("Failed to add tender");
            }
        }

        public async Task<List<Tender>> getRestOfTripsThathasTenders(Tender tender)
        {
           var list= await _db.tenders.AsNoTracking().Include(t=>t.Trip)
                .Where(t => t.TenderId != tender.TenderId && t.DriverId == tender.DriverId&&
                t.staute == TenderStatue.WaitingForPassenger && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
            return list;
        }

        public async Task<Tender?> GetTenderByIdAsync(Guid tenderId)
        {
            var tender = await _db.tenders.FirstOrDefaultAsync(t =>t.TenderId==tenderId);
            return tender;
        }

        public async Task<List<TenderDataResponse>> GetTendersByTripIdAsync(Guid tripId)
        {
            var list = await _db.tenders.AsNoTracking().Include(t => t.Trip)
               .Where(t => t.TripId == tripId &&t.staute==TenderStatue.WaitingForPassenger&&t.ExpiresAt>DateTime.UtcNow)
               .Select(t => new TenderDataResponse
               {
                   TenderId = t.TenderId,
                   DriverName = t.Driver.UserName!,
                   OfferedPrice = t.OfferedPrice,
                   DriverRating = t.Driver.rating,
                   DriverPhoneNumber = t.Driver.PhoneNumber!,
                   licensePlate = t.Driver.LicensePlate,
                   ExpiryTime = t.ExpiresAt

               })
               .ToListAsync();
            return list;
        }

        public async Task<bool> isThereActiveDriverTenderForThisTrip(Guid tripId, string driverId)
        {
            return await _db.tenders.AnyAsync(t=>t.TripId==tripId&&t.DriverId==driverId
            &&(t.staute==TenderStatue.WaitingForPassenger|| t.staute == TenderStatue.WaitingForDriverConfirimation)&&t.ExpiresAt>DateTime.UtcNow);
        }

      

        public async Task updateRestOfActiveTenders(Tender tender)
        {
            await _db.tenders.Where(t => t.TenderId != tender.TenderId && t.DriverId == tender.DriverId && t.ExpiresAt > DateTime.UtcNow).
                ExecuteUpdateAsync(s=>s.SetProperty(t=>t.staute,TenderStatue.driverGotAnotherTrip));
        }

        public async Task<bool> updateTender(Tender tender)
        {
            var exsitingtender = await _db.tenders.FirstOrDefaultAsync(t => t.TripId == tender.TripId && t.DriverId == tender.DriverId);
            exsitingtender.staute = tender.staute;
            exsitingtender.OfferedPrice = tender.OfferedPrice;
            exsitingtender.ExpiresAt = tender.ExpiresAt;
            _db.tenders.Update(exsitingtender);
            var res = await _db.SaveChangesAsync() > 0;
            return res ;
        }
    }
}
