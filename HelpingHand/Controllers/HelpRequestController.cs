using HelpingHand.Models;
using HelpingHand.Repositories;
using HelpingHand.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpingHand.Controllers
{
    public class HelpRequestController : Controller
    {
        private readonly IHelpRequestRepository _requestRepo;
        private readonly ICategoryRepository _categoryRepo;

        public HelpRequestController(
            IHelpRequestRepository requestRepo,
            ICategoryRepository categoryRepo)
        {
            _requestRepo = requestRepo;
            _categoryRepo = categoryRepo;
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var model = new CreateHelpRequestViewModel
            {
                Categories = await _categoryRepo.GetAllAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateHelpRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryRepo.GetAllAsync();
                return View(model);
            }

            var request = new HelpRequest
            {
                Title = model.Title,
                Description = model.Description,
                CategoryId = model.CategoryId,
                PreferredDate = model.PreferredDate,
                Status = RequestStatus.Open,
                RequesterId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                CreatedAt = DateTime.UtcNow
            };

            await _requestRepo.AddAsync(request);
            await _requestRepo.SaveChangesAsync();

            TempData["Success"] = "Your request has been posted to the Help Board.";
            return RedirectToAction("Board", "Home");
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Claim(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            if (request.Status != RequestStatus.Open)
            {
                TempData["Error"] = "This request is no longer available to claim.";
                return RedirectToAction("Board", "Home");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (request.RequesterId == userId)
            {
                TempData["Error"] = "You cannot claim your own request.";
                return RedirectToAction(nameof(Details), new { id });
            }

            request.Status = RequestStatus.Assigned;
            request.VolunteerId = userId;

            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            TempData["Success"] = "You have successfully claimed this request.";
            return RedirectToAction("Dashboard", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Complete(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            if (request.Status != RequestStatus.Assigned)
            {
                TempData["Error"] = "Only assigned requests can be marked as complete.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (request.VolunteerId != userId && request.RequesterId != userId)
                return Forbid();

            request.Status = RequestStatus.Completed;

            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            TempData["Success"] = "This request has been marked as completed!";
            return RedirectToAction("Dashboard", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (request.RequesterId != userId)
                return Forbid();

            if (request.Status != RequestStatus.Open)
            {
                TempData["Error"] = "Only open requests can be cancelled.";
                return RedirectToAction(nameof(Details), new { id });
            }

            request.Status = RequestStatus.Cancelled;

            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            TempData["Success"] = "Your request has been cancelled.";
            return RedirectToAction("Dashboard", "Account");
        }
    }
}