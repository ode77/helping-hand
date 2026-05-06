using HelpingHand.Models;

namespace HelpingHand.Repositories
{
    public interface IHelpRequestRepository
    {
        Task<IEnumerable<HelpRequest>> GetOpenRequestsAsync();
        Task<IEnumerable<HelpRequest>> GetAllAsync();
        Task<HelpRequest?> GetByIdAsync(int id);
        Task<IEnumerable<HelpRequest>> GetByRequesterIdAsync(string userId);
        Task<IEnumerable<HelpRequest>> GetByVolunteerIdAsync(string userId);
        Task AddAsync(HelpRequest request);
        Task UpdateAsync(HelpRequest request);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}