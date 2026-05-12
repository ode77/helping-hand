using HelpingHand.Models;

namespace HelpingHand.ViewModels
{
    public class NotificationViewModel
    {
        public IEnumerable<Notification> Notifications { get; set; }
            = Enumerable.Empty<Notification>();
        public int UnreadCount { get; set; }
    }
}