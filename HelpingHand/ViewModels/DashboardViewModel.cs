using HelpingHand.Models;

namespace HelpingHand.ViewModels
{
    public class DashboardViewModel
    {
        public string UserFullName { get; set; } = string.Empty;
        public IEnumerable<HelpRequest> PostedRequests { get; set; }
            = Enumerable.Empty<HelpRequest>();
        public IEnumerable<HelpRequest> ClaimedRequests { get; set; }
            = Enumerable.Empty<HelpRequest>();
        public IEnumerable<HelpRequest> CompletedRequests { get; set; }
            = Enumerable.Empty<HelpRequest>();
    }
}