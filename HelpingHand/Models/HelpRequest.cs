using System.ComponentModel.DataAnnotations;

namespace HelpingHand.Models
{
    public class HelpRequest
    {
        public int HelpRequestId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public RequestStatus Status { get; set; } = RequestStatus.Open;

        public DateTime CreatedAt { get; set; }

        public DateTime? PreferredDate { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public string RequesterId { get; set; } = string.Empty;
        public ApplicationUser? Requester { get; set; }

        public string? VolunteerId { get; set; }
        public ApplicationUser? Volunteer { get; set; }
    }
}