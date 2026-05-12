using System.ComponentModel.DataAnnotations;

namespace HelpingHand.ViewModels
{
    public class VolunteerApplicationViewModel
    {
        public int HelpRequestId { get; set; }
        public string RequestTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please write a message.")]
        [MaxLength(500)]
        [Display(Name = "Why can you help with this request?")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please upload a photo of your ID.")]
        [Display(Name = "ID Document (photo or scan)")]
        public IFormFile? IdDocument { get; set; }
    }
}