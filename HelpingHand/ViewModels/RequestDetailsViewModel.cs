using HelpingHand.Models;

namespace HelpingHand.ViewModels
{
    public class RequestDetailsViewModel
    {
        public HelpRequest Request { get; set; } = null!;
        public IEnumerable<VolunteerApplication> Applications { get; set; }
            = Enumerable.Empty<VolunteerApplication>();
        public IEnumerable<RequestComment> Comments { get; set; }
            = Enumerable.Empty<RequestComment>();
        public bool AlreadyApplied { get; set; }
        public bool AlreadyRated { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
    }
}