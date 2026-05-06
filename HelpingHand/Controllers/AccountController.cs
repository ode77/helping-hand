using HelpingHand.Models;
using HelpingHand.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HelpingHand.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Dashboard");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

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

        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;

            var user = await _userManager.FindByIdAsync(userId);

            var requestRepo = HttpContext.RequestServices
                .GetRequiredService<HelpingHand.Repositories.IHelpRequestRepository>();

            var model = new DashboardViewModel
            {
                UserFullName = user?.FullName ?? "User",
                PostedRequests = (await requestRepo.GetByRequesterIdAsync(userId))
                    .Where(r => r.Status != RequestStatus.Completed
                             && r.Status != RequestStatus.Cancelled),
                ClaimedRequests = (await requestRepo.GetByVolunteerIdAsync(userId))
                    .Where(r => r.Status == RequestStatus.Assigned),
                CompletedRequests = (await requestRepo.GetByRequesterIdAsync(userId))
                    .Where(r => r.Status == RequestStatus.Completed)
            };

            return View(model);
        }

        public IActionResult AccessDenied() => View();
    }
}