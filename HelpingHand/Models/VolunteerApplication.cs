using System.ComponentModel.DataAnnotations;

namespace HelpingHand.Models
{
    public class VolunteerApplication
    {
        public int VolunteerApplicationId { get; set; }

        public int HelpRequestId { get; set; }
        public HelpRequest? HelpRequest { get; set; }

        [Required]
        public string VolunteerId { get; set; } = string.Empty;
        public ApplicationUser? Volunteer { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        public DateTime AppliedAt { get; set; }

        public bool IsAccepted { get; set; } = false;

        // ID verification
        [MaxLength(300)]
        public string IdDocumentPath { get; set; } = string.Empty;

        public bool IdVerified { get; set; } = false;
    }
}