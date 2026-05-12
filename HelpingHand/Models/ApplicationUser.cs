using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HelpingHand.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(20)]
        [Display(Name = "Phone Number")]
        public new string PhoneNumber { get; set; } = string.Empty;

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

        // Categories this volunteer can help with
        public string SkillCategories { get; set; } = string.Empty;

        // Badge tier based on completions
        public int CompletedHelpCount { get; set; } = 0;

        public ICollection<HelpRequest> PostedRequests { get; set; }
            = new List<HelpRequest>();

        public ICollection<HelpRequest> ClaimedRequests { get; set; }
            = new List<HelpRequest>();

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();

        public ICollection<VolunteerRating> RatingsReceived { get; set; }
            = new List<VolunteerRating>();

        // Computed badge based on completion count
        public string Badge => CompletedHelpCount switch
        {
            >= 50 => "Champion",
            >= 25 => "Trusted Volunteer",
            >= 10 => "Regular Helper",
            >= 5 => "Community Helper",
            _ => ""
        };
    }
}