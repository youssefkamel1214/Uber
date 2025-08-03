using Microsoft.EntityFrameworkCore;
using Uber.Data;
using Uber.Models.Domain;
using Uber.Repositories.Interfaces;

namespace Uber.Repositories
{
    public class ReviewRepository:IReviewRepository
    {
        private readonly UberAuthDatabase _db;
        public ReviewRepository(UberAuthDatabase uberDatabase)
        {
            _db = uberDatabase;
        }

        public async Task<Review> addReview(Review review)
        {
            review.ReviewId = Guid.NewGuid();
            await _db.reviews.AddAsync(review);
            var result = await _db.SaveChangesAsync();
            if (result > 0)
            {
                return review;
            }
            else
            {
                throw new Exception("Failed to add review");
            }
        }

        public async Task<Review?> getReviewById(Guid Rid)
        {
            var review = await _db.reviews.FirstOrDefaultAsync(r => r.ReviewId == Rid);
            return review;
        }

        public async Task<List<Review>> getReviewsByDriverId(string driverId)
        {
            var reviews = await _db.reviews.Where(r => _db.trips.Any(tr => tr.DriverId == driverId && tr.TripId == r.TripId)).ToListAsync();
            return reviews;
        }

        public async Task<List<Review>> getReviewsByPassengerId(string passengerId)
        {
            var reviews = await _db.reviews.Where(r => _db.trips.Any(tr => tr.PassengerId == passengerId && tr.TripId == r.TripId)).ToListAsync();
            return reviews;
        }
    }
}
