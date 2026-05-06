using HelpingHand.Models;
using HelpingHand.Repositories;
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

        public AdminController(
            IHelpRequestRepository requestRepo,
            UserManager<ApplicationUser> userManager)
        {
            _requestRepo = requestRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var all = await _requestRepo.GetAllAsync();
            return View(all);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            await _requestRepo.DeleteAsync(id);
            await _requestRepo.SaveChangesAsync();
            TempData["Success"] = "Request deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}