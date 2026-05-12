using HelpingHand.Data;
using HelpingHand.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpingHand.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        public NotificationRepository(ApplicationDbContext context)
            => _context = context;

        public async Task AddAsync(Notification notification)
            => await _context.Notifications.AddAsync(notification);

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(
            string userId)
            => await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(20)
                .ToListAsync();

        public async Task MarkAllReadAsync(string userId)
        {
            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();
            unread.ForEach(n => n.IsRead = true);
        }

        public async Task<int> GetUnreadCountAsync(string userId)
            => await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}