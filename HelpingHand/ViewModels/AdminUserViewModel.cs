namespace HelpingHand.ViewModels
{
    public class AdminUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int TotalRequestsPosted { get; set; }
        public int TotalRequestsClaimed { get; set; }
        public int TotalCompleted { get; set; }
    }
}