using HelpingHand.Models;
using HelpingHand.Repositories;
using HelpingHand.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HelpingHand.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IHelpRequestRepository _requestRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationRepository _notifRepo;
        private readonly IVolunteerApplicationRepository _appRepo;

        public AdminController(
            IHelpRequestRepository requestRepo,
            UserManager<ApplicationUser> userManager,
            INotificationRepository notifRepo,
            IVolunteerApplicationRepository appRepo)
        {
            _requestRepo = requestRepo;
            _userManager = userManager;
            _notifRepo = notifRepo;
            _appRepo = appRepo;
        }

        // Main dashboard
        public async Task<IActionResult> Index()
        {
            var all = await _requestRepo.GetAllAsync();
            return View(all);
        }

        // View full requester contact details
        public async Task<IActionResult> RequesterDetails(
            int requestId)
        {
            var request = await _requestRepo
                .GetByIdAsync(requestId);
            if (request == null) return NotFound();

            var requester = await _userManager.FindByIdAsync(request.RequesterId);
            if (requester == null) return NotFound();

            var model = new AdminUserViewModel
            {
                Id = requester.Id,
                FullName = requester.FullName,
                Email = requester.Email ?? string.Empty,
                PhoneNumber = requester.PhoneNumber
                    ?? string.Empty,
                Address = requester.Address,
                Availability = requester.Availability,
                EmergencyContactName =
                    requester.EmergencyContactName,
                EmergencyContactPhone =
                    requester.EmergencyContactPhone,
            };

            ViewBag.RequestId = requestId;
            ViewBag.RequestTitle = request.Title;
            return View(model);
        }

        // Approve a pending claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveClaim(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            if (request.Status != RequestStatus.PendingApproval)
            {
                TempData["Error"] =
                    "This request is not pending approval.";
                return RedirectToAction(nameof(Index));
            }

            request.Status = RequestStatus.Assigned;
            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            // Notify volunteer
            if (request.VolunteerId != null)
            {
                await _notifRepo.AddAsync(new Notification
                {
                    UserId = request.VolunteerId,
                    Message = $"Your claim for '{request.Title}' " +
                              "has been approved. You are now assigned!",
                    CreatedAt = DateTime.UtcNow,
                    RelatedRequestId = request.HelpRequestId
                });
                await _notifRepo.SaveChangesAsync();
            }

            TempData["Success"] =
                "Claim approved. Volunteer assigned.";
            return RedirectToAction(nameof(Index));
        }

        // Reject a pending claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectClaim(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            if (request.Status != RequestStatus.PendingApproval)
            {
                TempData["Error"] =
                    "This request is not pending approval.";
                return RedirectToAction(nameof(Index));
            }

            request.Status = RequestStatus.Open;
            request.VolunteerId = null;
            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            TempData["Success"] =
                "Claim rejected. Request returned to Open.";
            return RedirectToAction(nameof(Index));
        }

        // View volunteer ID
        public async Task<IActionResult> ViewVolunteerId(
            int applicationId)
        {
            var application = await _appRepo.GetByIdAsync(applicationId);
            if (application == null) return NotFound();

            ViewBag.Application = application;
            ViewBag.VolunteerName = application.Volunteer?.FullName ?? "Volunteer";
            ViewBag.IdPath = application.IdDocumentPath;
            return View();
        }

        // Verify volunteer ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyId(
            int applicationId)
        {
            var application = await _appRepo.GetByIdAsync(applicationId);
            if (application == null) return NotFound();

            application.IdVerified = true;
            await _appRepo.SaveChangesAsync();

            TempData["Success"] = "ID verified. You can now approve the claim.";
            return RedirectToAction(nameof(Index));
        }

        // Manually assign a volunteer
        public async Task<IActionResult> AssignVolunteer(
            int requestId)
        {
            var request = await _requestRepo
                .GetByIdAsync(requestId);
            if (request == null) return NotFound();

            var users = _userManager.Users
                .Where(u => u.Id != request.RequesterId)
                .ToList();

            ViewBag.RequestId = requestId;
            ViewBag.RequestTitle = request.Title;
            ViewBag.Users = users;
            return View();
        }

        // POST: /Admin/AssignVolunteer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignVolunteer(int requestId, string volunteerId)
        {
            var request = await _requestRepo
                .GetByIdAsync(requestId);
            if (request == null) return NotFound();

            request.VolunteerId = volunteerId;
            request.Status = RequestStatus.Assigned;

            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            TempData["Success"] = "Volunteer assigned successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/EditRequest/5
        public async Task<IActionResult> EditRequest(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();
            return View(request);
        }

        // POST: /Admin/EditRequest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRequest(int id, string title, string description)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            request.Title = title;
            request.Description = description;

            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            TempData["Success"] = "Request updated.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/DeleteRequest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            await _requestRepo.DeleteAsync(id);
            await _requestRepo.SaveChangesAsync();

            TempData["Success"] = "Request deleted.";
            return RedirectToAction(nameof(Index));
        }

        // All users with contact info
        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var models = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager
                    .GetRolesAsync(user);
                var posted = await _requestRepo
                    .GetByRequesterIdAsync(user.Id);
                var claimed = await _requestRepo
                    .GetByVolunteerIdAsync(user.Id);

                models.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber
                        ?? string.Empty,
                    Address = user.Address,
                    Availability = user.Availability,
                    EmergencyContactName =
                        user.EmergencyContactName,
                    EmergencyContactPhone =
                        user.EmergencyContactPhone,
                    Role = roles.FirstOrDefault() ?? "User",
                    TotalRequestsPosted = posted.Count(),
                    TotalRequestsClaimed = claimed.Count(),
                    TotalCompleted = claimed.Count(
                        r => r.Status == RequestStatus.Completed)
                });
            }

            return View(models);
        }

        // POST: /Admin/ToggleAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAdmin(
            string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.RemoveFromRoleAsync(
                    user, "Admin");
                await _userManager.AddToRoleAsync(user, "User");
                TempData["Success"] =
                    $"{user.FullName} removed from Admin role.";
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(
                    user, "User");
                await _userManager.AddToRoleAsync(user, "Admin");
                TempData["Success"] =
                    $"{user.FullName} is now an Admin.";
            }

            return RedirectToAction(nameof(ManageUsers));
        }
    }
}