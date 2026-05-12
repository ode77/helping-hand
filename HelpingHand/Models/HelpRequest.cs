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

        public RequestUrgency Urgency { get; set; } = RequestUrgency.Medium;

        public DateTime CreatedAt { get; set; }

        public DateTime? PreferredDate { get; set; }

        // Expires after 14 days if not claimed
        public DateTime ExpiresAt { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public string RequesterId { get; set; } = string.Empty;
        public ApplicationUser? Requester { get; set; }

        public string? VolunteerId { get; set; }
        public ApplicationUser? Volunteer { get; set; }

        // Dual confirmation tracking
        public bool VolunteerConfirmedDone { get; set; } = false;
        public bool RequesterConfirmedDone { get; set; } = false;

        // Requester feedback on completion
        [MaxLength(500)]
        public string RequesterFeedback { get; set; } = string.Empty;

        // Navigation
        public ICollection<VolunteerApplication> Applications { get; set; }
            = new List<VolunteerApplication>();

        public ICollection<RequestComment> Comments { get; set; }
            = new List<RequestComment>();

        public ICollection<VolunteerRating> Ratings { get; set; }
            = new List<VolunteerRating>();
    }
}