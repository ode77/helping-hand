using System.ComponentModel.DataAnnotations;

namespace HelpingHand.Models
{
    public class RequestComment
    {
        public int RequestCommentId { get; set; }

        public int HelpRequestId { get; set; }
        public HelpRequest? HelpRequest { get; set; }

        [Required]
        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser? Author { get; set; }

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        public DateTime PostedAt { get; set; }
    }
}