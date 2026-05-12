using HelpingHand.Models;

namespace HelpingHand.Repositories
{
    public interface IVolunteerApplicationRepository
    {
        Task AddAsync(VolunteerApplication application);
        Task<IEnumerable<VolunteerApplication>> GetByRequestIdAsync(int requestId);
        Task<VolunteerApplication?> GetByIdAsync(int id);
        Task SaveChangesAsync();
    }
}