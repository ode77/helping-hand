using System.ComponentModel.DataAnnotations;

namespace HelpingHand.Models
{
    public class VolunteerRating
    {
        public int VolunteerRatingId { get; set; }

        public int HelpRequestId { get; set; }
        public HelpRequest? HelpRequest { get; set; }

        [Required]
        public string VolunteerId { get; set; } = string.Empty;
        public ApplicationUser? Volunteer { get; set; }

        [Required]
        public string RequesterId { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Stars { get; set; }

        [MaxLength(300)]
        public string Comment { get; set; } = string.Empty;

        public DateTime RatedAt { get; set; }
    }
}