using System.ComponentModel.DataAnnotations;

namespace HelpingHand.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<HelpRequest> HelpRequests { get; set; }
            = new List<HelpRequest>();
    }
}