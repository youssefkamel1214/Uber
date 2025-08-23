
using Microsoft.EntityFrameworkCore;
using Uber.Data;
using Uber.Models.Domain;
using Uber.Repositories.Interfaces;

namespace Uber.Repositories
{
    public class PassengerRepository : IPassengerRepository
    {
        private readonly UberAuthDatabase _uberAuthDatabase;

        public PassengerRepository(UberAuthDatabase uberAuthDatabase)
        {
            _uberAuthDatabase = uberAuthDatabase;
        }

        public async Task<bool> createPassengerAsync(Passenger passenger)
        {

            var pass = await _uberAuthDatabase.UberUsers.AddAsync(passenger);
            if (pass == null)
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
                if (ex.InnerException.Message.ToLower().Contains("unique"))
                    throw new InvalidOperationException("Passenger Data already exists Duplicates could be (Licenese,ssn,phone number).");
                throw ex;

            }
        }

        public async Task<Passenger?> getPassengerByIdAsync(string passengerId)
        {
            var passenger =await  _uberAuthDatabase
                .UberUsers.OfType<Passenger>().FirstOrDefaultAsync(p => p.Id == passengerId);
            return passenger;
        }

        public async Task<bool> updatePassenger(Passenger passenger)
        {
            var exsitingPassenger = await _uberAuthDatabase
                         .UberUsers.OfType<Passenger>().FirstOrDefaultAsync(d => d.Id == passenger.Id);
            if (exsitingPassenger == null)
            {
                throw new InvalidOperationException("Passenger not found.");
            }
            exsitingPassenger.rating = passenger.rating;
            exsitingPassenger.NumberOfReviews = passenger.NumberOfReviews;
            exsitingPassenger.Home = passenger.Home;
            exsitingPassenger.Work = passenger.Work;
            _uberAuthDatabase.UberUsers.Update(exsitingPassenger);
            return await _uberAuthDatabase.SaveChangesAsync()>0;

        }
    }
}
