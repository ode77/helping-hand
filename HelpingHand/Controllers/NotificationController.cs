using HelpingHand.Repositories;
using HelpingHand.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpingHand.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationRepository _notifRepo;

        public NotificationController(INotificationRepository notifRepo)
            => _notifRepo = notifRepo;

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier)!;

            var notifications = await _notifRepo
                .GetByUserIdAsync(userId);

            await _notifRepo.MarkAllReadAsync(userId);
            await _notifRepo.SaveChangesAsync();

            var model = new NotificationViewModel
            {
                Notifications = notifications,
                UnreadCount = 0
            };

            return View(model);
        }
    }
}