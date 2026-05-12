using System.ComponentModel.DataAnnotations;

namespace HelpingHand.Models
{
    public class RequestTemplate
    {
        public int RequestTemplateId { get; set; }

        [Required]
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser? Owner { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public RequestUrgency Urgency { get; set; } = RequestUrgency.Medium;

        public DateTime CreatedAt { get; set; }
    }
}