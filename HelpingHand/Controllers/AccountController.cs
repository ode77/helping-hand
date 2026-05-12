using HelpingHand.Models;
using HelpingHand.Repositories;
using HelpingHand.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HelpingHand.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHelpRequestRepository _requestRepo;
        private readonly ITemplateRepository _templateRepo;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHelpRequestRepository requestRepo,
            ITemplateRepository templateRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _requestRepo = requestRepo;
            _templateRepo = templateRepo;
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                Availability = model.Availability,
                EmergencyContactName = model.EmergencyContactName,
                EmergencyContactPhone = model.EmergencyContactPhone
            };

            var result = await _userManager
                .CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager
                    .SignInAsync(user, isPersistent: false);
                return RedirectToAction("Dashboard");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(
                    string.Empty, error.Description);

            return View(model);
        }

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password,
                model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
                return RedirectToAction("Dashboard");

            if (result.IsLockedOut)
                ModelState.AddModelError(string.Empty,
                    "Account locked. Try again in 15 minutes.");
            else
                ModelState.AddModelError(string.Empty,
                    "Invalid email or password.");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirst(
                System.Security.Claims.ClaimTypes
                    .NameIdentifier)?.Value!;

            var user = await _userManager.FindByIdAsync(userId);

            var templates = await _templateRepo
                .GetByOwnerIdAsync(userId);

            var model = new DashboardViewModel
            {
                UserFullName = user?.FullName ?? "User",
                Badge = user?.Badge ?? string.Empty,
                PostedRequests = (await _requestRepo
                    .GetByRequesterIdAsync(userId))
                    .Where(r =>
                        r.Status != RequestStatus.Completed &&
                        r.Status != RequestStatus.Cancelled &&
                        r.Status != RequestStatus.Expired),
                ClaimedRequests = (await _requestRepo
                    .GetByVolunteerIdAsync(userId))
                    .Where(r =>
                        r.Status == RequestStatus.Assigned ||
                        r.Status == RequestStatus.PendingApproval ||
                        r.Status == RequestStatus.VolunteerDone),
                CompletedRequests = (await _requestRepo
                    .GetByRequesterIdAsync(userId))
                    .Where(r =>
                        r.Status == RequestStatus.Completed),
                Templates = templates
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirst(
                System.Security.Claims.ClaimTypes
                    .NameIdentifier)?.Value!;
            var user = await _userManager.FindByIdAsync(userId);

            var model = new ProfileViewModel
            {
                FullName = user?.FullName ?? string.Empty,
                PhoneNumber = user?.PhoneNumber ?? string.Empty,
                Address = user?.Address ?? string.Empty,
                Availability = user?.Availability ?? string.Empty,
                EmergencyContactName =
                    user?.EmergencyContactName ?? string.Empty,
                EmergencyContactPhone =
                    user?.EmergencyContactPhone ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.FindFirst(
                System.Security.Claims.ClaimTypes
                    .NameIdentifier)?.Value!;
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.Availability = model.Availability;
            user.EmergencyContactName = model.EmergencyContactName;
            user.EmergencyContactPhone = model.EmergencyContactPhone;

            await _userManager.UpdateAsync(user);

            TempData["Success"] =
                "Your profile has been updated.";
            return RedirectToAction(nameof(Profile));
        }

        public IActionResult AccessDenied() => View();
    }
}