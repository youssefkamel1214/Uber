using Uber.Models.Domain;

namespace Uber.Repositories.Interfaces
{
    public interface IDriverRepository
    {
        public Task<bool> createDriverAsync(Driver driver);
        public Task<Driver?> getDriverByIdAsync(string driverId);
        public Task<bool> updateDriver(Driver driver);
    }
}
