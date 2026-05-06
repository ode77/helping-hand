using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HelpingHand.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public ICollection<HelpRequest> PostedRequests { get; set; }
            = new List<HelpRequest>();

        public ICollection<HelpRequest> ClaimedRequests { get; set; }
            = new List<HelpRequest>();
    }
}