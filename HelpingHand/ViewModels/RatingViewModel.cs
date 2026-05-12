using System.ComponentModel.DataAnnotations;

namespace HelpingHand.ViewModels
{
    public class RatingViewModel
    {
        public int HelpRequestId { get; set; }
        public string VolunteerName { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Please select a rating.")]
        public int Stars { get; set; }

        [MaxLength(300)]
        [Display(Name = "Comment (optional)")]
        public string Comment { get; set; } = string.Empty;
    }
}