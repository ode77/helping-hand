using System.ComponentModel.DataAnnotations;

namespace HelpingHand.ViewModels
{
    public class ProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [MaxLength(250)]
        [Display(Name = "Availability")]
        public string Availability { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Emergency Contact Name")]
        public string EmergencyContactName { get; set; } = string.Empty;

        [MaxLength(20)]
        [Display(Name = "Emergency Contact Phone")]
        public string EmergencyContactPhone { get; set; } = string.Empty;
    }
}