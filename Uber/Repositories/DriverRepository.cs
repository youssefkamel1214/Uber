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
            var dr= await _uberAuthDatabase.drivers.AddAsync(driver);
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
                .drivers.FirstOrDefaultAsync(d => d.DriverId == driverId);
            return driver;
        }

        public async Task<bool> updateDriver(Driver driver)
        {
            var exsitingDriver = await _uberAuthDatabase
                       .drivers.FirstOrDefaultAsync(d => d.DriverId == driver.DriverId);
            if (exsitingDriver == null)
            {
                throw new InvalidOperationException("Driver not found.");
            }
            exsitingDriver.isAvailable = driver.isAvailable;
            exsitingDriver.IsActive = driver.IsActive;
            exsitingDriver.Rating = driver.Rating;
          return  await _uberAuthDatabase.SaveChangesAsync()>0;
        }
    }
}
