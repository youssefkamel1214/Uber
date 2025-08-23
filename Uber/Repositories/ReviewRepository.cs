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

        public async Task<bool> getIfThereReviewForSameTrip(Guid tripId, string UserId)
        {
            return await _db.reviews.AnyAsync(r => r.TripId == tripId && r.ReviewerId == UserId);
        }

        public async Task<Review?> getReviewById(Guid Rid)
        {
            var review = await _db.reviews.FirstOrDefaultAsync(r => r.ReviewId == Rid);
            return review;
        }

        public async Task<List<Review>> getReviewsByDriverId(string driverId)
        {
            var reviews = await (from rv in _db.reviews
                                 join tr in _db.trips on rv.TripId equals tr.TripId
                                 where tr.DriverId == driverId
                                 select rv).ToListAsync();
            return reviews;
        }

        public async Task<List<Review>> getReviewsByPassengerId(string passengerId)
        {
            var reviews = await _db.reviews.Join(_db.trips,
                rv=>rv.TripId,
                tr=>tr.TripId,
                (rv,tr)=>new {Review=rv,Trip=tr })
                .Where(r => r.Trip.PassengerId==passengerId).Select(row=>row.Review).ToListAsync();
            return reviews;
        }

        public async Task<List<Review>> GetReviewsOnTrip(Guid tripId)
        {
            var reviews = await _db.reviews.Where(r => r.TripId == tripId).ToListAsync();
            return reviews;
        }
    }
}
