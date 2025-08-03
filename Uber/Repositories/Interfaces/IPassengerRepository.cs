using Uber.Models.Domain;

namespace Uber.Repositories.Interfaces
{
    public interface IPassengerRepository
    {
        public Task<bool> createPassengerAsync(Passenger passenger);
        public Task<Passenger?> getPassengerByIdAsync(string passengerId);
        public Task<bool> updatePassenger(Passenger passenger);
    }
}
