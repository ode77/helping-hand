using HelpingHand.Models;
using System.ComponentModel.DataAnnotations;

namespace HelpingHand.ViewModels
{
    public class CreateHelpRequestViewModel
    {
        [Required(ErrorMessage = "Please enter a title.")]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please describe what help you need.")]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Preferred Date (optional)")]
        public DateTime? PreferredDate { get; set; }

        [Display(Name = "Urgency Level")]
        public RequestUrgency Urgency { get; set; } = RequestUrgency.Medium;

        public IEnumerable<Category> Categories { get; set; }
            = Enumerable.Empty<Category>();

        // If created from a template
        public int? TemplateId { get; set; }
    }
}