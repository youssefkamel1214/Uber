using Microsoft.EntityFrameworkCore;
using Uber.Data;
using Uber.Models.Domain;
using Uber.Repositories.Interfaces;
using Uber.Utils;

namespace Uber.Repositories
{
    public class TenderRepository : ITenderRepository
    {
        private readonly UberAuthDatabase _db;
        private readonly IServiceProvider _serviceProvider;

        public TenderRepository(UberAuthDatabase db, IServiceProvider serviceProvider)
        {
            _db = db;
            _serviceProvider = serviceProvider;
        }

        public async Task<Tender> AddTender(Tender tender)
        {
            tender.TenderId = Guid.NewGuid();
            await _db.tenders.AddAsync(tender);
            var result = await _db.SaveChangesAsync();
            if (result > 0)
            {
             makeTenderExpire(tender);
             return tender;
            }
            else
            {
             throw new Exception("Failed to add tender");
            }
        }

        public async Task<Tender?> GetTenderByIdAsync(Guid tenderId)
        {
            var tender = await _db.tenders.FirstOrDefaultAsync(t =>t.TenderId==tenderId);
            return tender;
        }

        public async Task<List<Tender>> GetTendersByTripIdAsync(Guid tripId)
        {
           
            var tenders =await  _db.tenders.Where(t => t.TripId == tripId&&t.TenderTime.AddMinutes(2)>DateTime.UtcNow).ToListAsync();
            return tenders;
        }

        public async Task<bool> isThereActiveDriverTenderForThisTrip(Guid tripId, string driverId)
        {
            return await _db.tenders.AnyAsync(t=>t.TripId==tripId&&t.DriverId==driverId
            &&(t.staute==TenderStatue.WaitingForPassenger|| t.staute == TenderStatue.WaitingForDriverConfirimation));
        }

        public async Task makeTenderExpire(Tender tender)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                {
                    var db = scope.ServiceProvider.GetRequiredService<UberAuthDatabase>();
                    await Task.Delay(5*60 * 1000);
               
                        var exstingTender = await db.tenders.FirstOrDefaultAsync(t => t.TenderId == tender.TenderId);
                        if (exstingTender == null)
                        {
                            return;
                        }

                        if (exstingTender.staute == TenderStatue.WaitingForPassenger)
                        {
                            exstingTender.staute = TenderStatue.Expired;
                            db.tenders.Update(exstingTender);
                            await db.SaveChangesAsync();
                        }
               
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task timeOutTenderConfirm(Tender tender)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                {
                    var db = scope.ServiceProvider.GetRequiredService<UberAuthDatabase>();
                    await Task.Delay(1000 * 60);
                    var existingTender = await db.tenders.FirstOrDefaultAsync(t => t.TenderId == tender.TenderId);
                    var existingtrip = await db.trips.FirstOrDefaultAsync(tr => tr.TripId == tender.TripId);
                    if (existingTender == null || existingtrip == null)
                        return;
                    if (existingtrip.Status == TripStatue.WaitingForConifirmationOnTender && existingTender.staute == TenderStatue.WaitingForDriverConfirimation)
                    {
                        existingtrip.Status = TripStatue.DriverWaiting;
                        db.trips.Update(existingtrip);
                        existingTender.staute = TenderStatue.Expired;
                        db.tenders.Update(existingTender);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        public async Task<bool> updateTender(Tender tender)
        {
            var exsitingtender = await _db.tenders.FirstOrDefaultAsync(t => t.TripId == tender.TripId && t.DriverId == tender.DriverId);
            exsitingtender.staute = tender.staute;
            _db.tenders.Update(exsitingtender);
            var res = await _db.SaveChangesAsync() > 0;
            if (res && tender.staute == TenderStatue.WaitingForDriverConfirimation)
                timeOutTenderConfirm(tender);
            return res ;
        }
    }
}
