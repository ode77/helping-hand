using HelpingHand.Models;
using HelpingHand.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpingHand.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHelpRequestRepository _requestRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            IHelpRequestRepository requestRepo,
            ICategoryRepository categoryRepo,
            UserManager<ApplicationUser> userManager)
        {
            _requestRepo = requestRepo;
            _categoryRepo = categoryRepo;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> Board(
            int? category = null,
            int? urgency = null)
        {
            var requests = await _requestRepo.GetOpenRequestsAsync();

            // Apply category filter
            if (category.HasValue)
                requests = requests
                    .Where(r => r.CategoryId == category.Value);

            // Apply urgency filter
            if (urgency.HasValue)
                requests = requests
                    .Where(r => (int)r.Urgency == urgency.Value);

            // Pass categories for filter dropdown
            var categories = await _categoryRepo.GetAllAsync();
            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;
            ViewBag.SelectedUrgency = urgency;

            // Matched requests for logged-in volunteers
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null &&
                    !string.IsNullOrEmpty(user.SkillCategories))
                {
                    var skillIds = user.SkillCategories
                        .Split(',')
                        .Select(s => int.TryParse(s.Trim(),
                            out int id) ? id : 0)
                        .Where(id => id > 0)
                        .ToList();

                    var matched = requests
                        .Where(r => skillIds
                            .Contains(r.CategoryId))
                        .Take(6);

                    ViewBag.MatchedRequests = matched;
                }
            }

            return View(requests);
        }
    }
}