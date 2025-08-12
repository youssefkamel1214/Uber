using Uber.Models.Domain;

namespace Uber.Repositories.Interfaces
{
    public interface IDriverRepository
    {
        public Task<bool> createDriverAsync(Driver driver);
        public Task<Driver?> getDriverByIdAsync(string driverId);
        public Task<bool> updateDriver(Driver driver);
        public Task<Driver?> MarkDriverIsActive(string email);
        public Task<bool> MarkDriverIsAvailble(string email);
    }
}
