using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uber.Data;
using Uber.Models.Domain;
using Uber.Repositories.Interfaces;

namespace Uber.Repositories
{
    public class DriverRepository : IDriverRepository
        
    {
        private readonly UberAuthDatabase _uberAuthDatabase;
        public DriverRepository(UberAuthDatabase uberAuthDatabase)
        {
            _uberAuthDatabase = uberAuthDatabase;
        }
        public async Task<bool> createDriverAsync(Driver driver)
        {
            var dr= await _uberAuthDatabase.UberUsers.AddAsync(driver);
            if (dr == null)
            {
                throw new InvalidOperationException("Driver could not be added.");
            }
            try
            {
             var res = await _uberAuthDatabase.SaveChangesAsync();
             return res > 0;

            }
            catch (Exception ex)
            {
                 if(ex.InnerException.Message.ToLower().Contains("unique"))
                    throw new InvalidOperationException("Driver Data already exists Duplicates could be (Licenese,ssn,phone number).");
                throw ex;
                
            }
        }

        public async Task<Driver?> getDriverByIdAsync(string driverId)
        {
            var driver = await _uberAuthDatabase
                .UberUsers.OfType<Driver>().FirstOrDefaultAsync(d => d.Id == driverId);
            return driver;
        }

        public async Task<Driver?> MarkDriverIsActive(string email)
        {

            var driver = await _uberAuthDatabase
                .UberUsers.OfType<Driver>().FirstOrDefaultAsync(d => d.Email == email);
            if (driver != null) 
            {
                driver.IsActive = true;                
                _uberAuthDatabase.UberUsers.Update(driver);
                await _uberAuthDatabase.SaveChangesAsync();
                await MarkDriverIsAvailble(driver.Id);
            }
            return driver;
        }

        public async Task<bool> MarkDriverIsAvailble(string driverId)
        {
            var driver = await _uberAuthDatabase
               .UberUsers.OfType<Driver>().FirstOrDefaultAsync(d => d.Id == driverId);
            if (driver != null)
            {
                driver.isAvailable = ! await  _uberAuthDatabase.trips.AnyAsync(tr => !(tr.Status == Utils.TripStatue.TripCancelled ||
                tr.Status == Utils.TripStatue.TripCompleted) && tr.DriverId == driver.Id);
                _uberAuthDatabase.UberUsers.Update(driver);
                await _uberAuthDatabase.SaveChangesAsync();
                return driver.isAvailable;
            }
            throw new Exception("driverId isnt Exists");
        }

        public async Task<bool> updateDriver(Driver driver)
        {
            var exsitingDriver = await _uberAuthDatabase
                       .UberUsers.OfType<Driver>().FirstOrDefaultAsync(d => d.Id == driver.Id);
            if (exsitingDriver == null)
            {
                throw new InvalidOperationException("Driver not found.");
            }
           exsitingDriver.NumberOfReviews = driver.NumberOfReviews;
            exsitingDriver.rating = driver.rating;
            _uberAuthDatabase.UberUsers.Update(exsitingDriver);
            return  await _uberAuthDatabase.SaveChangesAsync()>0;
        }
    }
}
