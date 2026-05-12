using HelpingHand.Models;

namespace HelpingHand.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<IEnumerable<Notification>> GetByUserIdAsync(string userId);
        Task MarkAllReadAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task SaveChangesAsync();
    }
}