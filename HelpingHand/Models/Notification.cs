using System.ComponentModel.DataAnnotations;

namespace HelpingHand.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required]
        [MaxLength(300)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public int? RelatedRequestId { get; set; }
    }
}