using HelpingHand.Models;

namespace HelpingHand.Repositories
{
    public interface ITemplateRepository
    {
        Task AddAsync(RequestTemplate template);
        Task<IEnumerable<RequestTemplate>> GetByOwnerIdAsync(string ownerId);
        Task<RequestTemplate?> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}