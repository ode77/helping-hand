using HelpingHand.Models;

namespace HelpingHand.Repositories
{
    public interface ICommentRepository
    {
        Task AddAsync(RequestComment comment);
        Task<IEnumerable<RequestComment>> GetByRequestIdAsync(int requestId);
        Task SaveChangesAsync();
    }
}