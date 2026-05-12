using HelpingHand.Models;

namespace HelpingHand.Repositories
{
    public interface IRatingRepository
    {
        Task AddAsync(VolunteerRating rating);
        Task<IEnumerable<VolunteerRating>> GetByVolunteerIdAsync(string volunteerId);
        Task<bool> ExistsAsync(int requestId, string requesterId);
        Task SaveChangesAsync();
    }
}