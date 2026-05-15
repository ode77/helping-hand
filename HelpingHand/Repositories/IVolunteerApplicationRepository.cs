using HelpingHand.Models;

namespace HelpingHand.Repositories
{
    public interface IVolunteerApplicationRepository
    {
        Task AddAsync(VolunteerApplication application);
        Task<IEnumerable<VolunteerApplication>> GetByRequestIdAsync(int requestId);
        Task<VolunteerApplication?> GetByIdAsync(int id);
        Task<VolunteerApplication?> GetByRequestAndVolunteerAsync(int requestId, string volunteerId);
        Task UpdateAsync(VolunteerApplication application);
        Task SaveChangesAsync();
    }
}