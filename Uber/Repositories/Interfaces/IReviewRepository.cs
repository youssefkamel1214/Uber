using Uber.Models.Domain;

namespace Uber.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        public Task<Review> addReview(Review review);
        public Task<Review?> getReviewById(Guid Rid);
        public Task<List<Review>> getReviewsByDriverId(string driverId);
        public Task<List<Review>> getReviewsByPassengerId(string passengerId);

    }
}
