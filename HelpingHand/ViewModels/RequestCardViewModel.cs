namespace HelpingHand.ViewModels
{
    public class RequestCardViewModel
    {
        public HelpingHand.Models.HelpRequest Request { get; set; }
            = null!;
        public string CurrentUserId { get; set; } = string.Empty;
    }
}