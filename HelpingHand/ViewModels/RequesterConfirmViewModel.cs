using System.ComponentModel.DataAnnotations;

namespace HelpingHand.ViewModels
{
    public class RequesterConfirmViewModel
    {
        public int HelpRequestId { get; set; }
        public string RequestTitle { get; set; } = string.Empty;
        public string VolunteerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please provide feedback before confirming.")]
        [MaxLength(500)]
        [Display(Name = "How did it go? Leave feedback for the volunteer.")]
        public string Feedback { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Please select a star rating.")]
        [Display(Name = "Star Rating")]
        public int Stars { get; set; } = 5;
    }
}