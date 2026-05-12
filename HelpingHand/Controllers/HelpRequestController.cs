using HelpingHand.Models;
using HelpingHand.Repositories;
using HelpingHand.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpingHand.Controllers
{
    public class HelpRequestController : Controller
    {
        private readonly IHelpRequestRepository _requestRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IVolunteerApplicationRepository _appRepo;
        private readonly INotificationRepository _notifRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly IRatingRepository _ratingRepo;
        private readonly ITemplateRepository _templateRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public HelpRequestController(
            IHelpRequestRepository requestRepo,
            ICategoryRepository categoryRepo,
            IVolunteerApplicationRepository appRepo,
            INotificationRepository notifRepo,
            ICommentRepository commentRepo,
            IRatingRepository ratingRepo,
            ITemplateRepository templateRepo,
            UserManager<ApplicationUser> userManager)
        {
            _requestRepo = requestRepo;
            _categoryRepo = categoryRepo;
            _appRepo = appRepo;
            _notifRepo = notifRepo;
            _commentRepo = commentRepo;
            _ratingRepo = ratingRepo;
            _templateRepo = templateRepo;
            _userManager = userManager;
        }

        // GET: Create — optionally pre-fill from template
        [Authorize]
        public async Task<IActionResult> Create(int? templateId = null)
        {
            var model = new CreateHelpRequestViewModel
            {
                Categories = await _categoryRepo.GetAllAsync()
            };

            if (templateId.HasValue)
            {
                var template = await _templateRepo
                    .GetByIdAsync(templateId.Value);
                if (template != null)
                {
                    model.Title = template.Title;
                    model.Description = template.Description;
                    model.CategoryId = template.CategoryId;
                    model.Urgency = template.Urgency;
                    model.TemplateId = templateId;
                }
            }

            return View(model);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(
            CreateHelpRequestViewModel model)
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
                Urgency = model.Urgency,
                Status = RequestStatus.Open,
                RequesterId = User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(14)
            };

            await _requestRepo.AddAsync(request);
            await _requestRepo.SaveChangesAsync();

            TempData["Success"] =
                "Your request has been posted to the Help Board.";
            return RedirectToAction("Board", "Home");
        }

        // GET: Details
        public async Task<IActionResult> Details(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comments = await _commentRepo.GetByRequestIdAsync(id);
            var applications = await _appRepo.GetByRequestIdAsync(id);
            var ratings = await _ratingRepo
                .GetByVolunteerIdAsync(request.VolunteerId ?? "");

            bool alreadyApplied = applications
                .Any(a => a.VolunteerId == userId);
            bool alreadyRated = userId != null &&
                await _ratingRepo.ExistsAsync(id, userId);

            double avgRating = ratings.Any()
                ? ratings.Average(r => r.Stars) : 0;

            var model = new RequestDetailsViewModel
            {
                Request = request,
                Applications = applications,
                Comments = comments,
                AlreadyApplied = alreadyApplied,
                AlreadyRated = alreadyRated,
                AverageRating = avgRating,
                RatingCount = ratings.Count()
            };

            return View(model);
        }

        // POST: Apply as volunteer with ID upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Apply(
            VolunteerApplicationViewModel model,
            IWebHostEnvironment env)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Details),
                    new { id = model.HelpRequestId });

            var request = await _requestRepo
                .GetByIdAsync(model.HelpRequestId);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier)!;

            if (request.RequesterId == userId)
            {
                TempData["Error"] =
                    "You cannot apply for your own request.";
                return RedirectToAction(nameof(Details),
                    new { id = model.HelpRequestId });
            }

            // Check not already applied
            var existing = await _appRepo
                .GetByRequestIdAsync(model.HelpRequestId);
            if (existing.Any(a => a.VolunteerId == userId))
            {
                TempData["Error"] =
                    "You have already applied for this request.";
                return RedirectToAction(nameof(Details),
                    new { id = model.HelpRequestId });
            }

            // Handle ID document upload
            string idPath = string.Empty;
            if (model.IdDocument != null &&
                model.IdDocument.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    env.WebRootPath, "uploads", "ids");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var ext = Path.GetExtension(
                    model.IdDocument.FileName);
                var fileName = $"{userId}_{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(
                    filePath, FileMode.Create))
                {
                    await model.IdDocument.CopyToAsync(stream);
                }

                idPath = $"/uploads/ids/{fileName}";
            }

            var application = new VolunteerApplication
            {
                HelpRequestId = model.HelpRequestId,
                VolunteerId = userId,
                Message = model.Message,
                AppliedAt = DateTime.UtcNow,
                IdDocumentPath = idPath
            };

            await _appRepo.AddAsync(application);
            await _appRepo.SaveChangesAsync();

            // Notify requester
            await _notifRepo.AddAsync(new Notification
            {
                UserId = request.RequesterId,
                Message = $"Someone has applied to help with your request: {request.Title}",
                CreatedAt = DateTime.UtcNow,
                RelatedRequestId = request.HelpRequestId
            });
            await _notifRepo.SaveChangesAsync();

            TempData["Success"] =
                "Your application has been submitted with your ID. " +
                "The requester will review applications.";
            return RedirectToAction("Dashboard", "Account");
        }

        // POST: Requester accepts a volunteer application
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AcceptApplication(
            int applicationId)
        {
            var application = await _appRepo.GetByIdAsync(applicationId);
            if (application == null) return NotFound();

            var request = await _requestRepo
                .GetByIdAsync(application.HelpRequestId);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (request.RequesterId != userId)
                return Forbid();

            application.IsAccepted = true;
            request.VolunteerId = application.VolunteerId;
            request.Status = RequestStatus.PendingApproval;

            await _appRepo.SaveChangesAsync();
            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            // Notify volunteer
            await _notifRepo.AddAsync(new Notification
            {
                UserId = application.VolunteerId,
                Message = $"Your application for '{request.Title}' was accepted and is awaiting admin approval.",
                CreatedAt = DateTime.UtcNow,
                RelatedRequestId = request.HelpRequestId
            });
            await _notifRepo.SaveChangesAsync();

            TempData["Success"] =
                "Volunteer accepted. Awaiting admin approval.";
            return RedirectToAction(nameof(Details),
                new { id = request.HelpRequestId });
        }

        // POST: Volunteer marks their side as done
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> VolunteerMarkDone(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (request.VolunteerId != userId)
                return Forbid();

            if (request.Status != RequestStatus.Assigned)
            {
                TempData["Error"] =
                    "This request is not in an assigned state.";
                return RedirectToAction(nameof(Details), new { id });
            }

            request.VolunteerConfirmedDone = true;
            request.Status = RequestStatus.VolunteerDone;

            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            // Notify requester to confirm
            await _notifRepo.AddAsync(new Notification
            {
                UserId = request.RequesterId,
                Message = $"Your volunteer says they have completed '{request.Title}'. Please confirm and leave feedback.",
                CreatedAt = DateTime.UtcNow,
                RelatedRequestId = request.HelpRequestId
            });
            await _notifRepo.SaveChangesAsync();

            TempData["Success"] =
                "You have marked this as done. " +
                "Waiting for the requester to confirm.";
            return RedirectToAction("Dashboard", "Account");
        }

        // GET: Requester confirms and leaves feedback
        [Authorize]
        public async Task<IActionResult> ConfirmComplete(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (request.RequesterId != userId)
                return Forbid();

            if (request.Status != RequestStatus.VolunteerDone)
            {
                TempData["Error"] =
                    "This request is not ready for confirmation yet.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var volunteer = request.VolunteerId != null
                ? await _userManager.FindByIdAsync(request.VolunteerId)
                : null;

            var model = new RequesterConfirmViewModel
            {
                HelpRequestId = id,
                RequestTitle = request.Title,
                VolunteerName = volunteer?.FullName ?? "Volunteer"
            };

            return View(model);
        }

        // POST: Requester confirms completion with feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ConfirmComplete(
            RequesterConfirmViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var request = await _requestRepo
                .GetByIdAsync(model.HelpRequestId);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier)!;

            if (request.RequesterId != userId)
                return Forbid();

            // Save feedback and rating
            request.RequesterConfirmedDone = true;
            request.RequesterFeedback = model.Feedback;
            request.Status = RequestStatus.Completed;

            await _requestRepo.UpdateAsync(request);

            // Save star rating
            if (request.VolunteerId != null)
            {
                var alreadyRated = await _ratingRepo
                    .ExistsAsync(model.HelpRequestId, userId);

                if (!alreadyRated)
                {
                    await _ratingRepo.AddAsync(new VolunteerRating
                    {
                        HelpRequestId = model.HelpRequestId,
                        VolunteerId = request.VolunteerId,
                        RequesterId = userId,
                        Stars = model.Stars,
                        Comment = model.Feedback,
                        RatedAt = DateTime.UtcNow
                    });
                }

                // Increment volunteer completion count
                var volunteer = await _userManager
                    .FindByIdAsync(request.VolunteerId);
                if (volunteer != null)
                {
                    volunteer.CompletedHelpCount++;
                    await _userManager.UpdateAsync(volunteer);
                }

                // Notify volunteer
                await _notifRepo.AddAsync(new Notification
                {
                    UserId = request.VolunteerId,
                    Message = $"'{request.Title}' has been confirmed as complete! " +
                              $"The requester rated you {model.Stars} stars.",
                    CreatedAt = DateTime.UtcNow,
                    RelatedRequestId = request.HelpRequestId
                });
                await _notifRepo.SaveChangesAsync();
            }

            await _requestRepo.SaveChangesAsync();

            TempData["Success"] =
                "Thank you for confirming! The request has been fully completed.";
            return RedirectToAction("Dashboard", "Account");
        }

        // POST: Cancel request
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
                TempData["Error"] =
                    "Only open requests can be cancelled.";
                return RedirectToAction(nameof(Details), new { id });
            }

            request.Status = RequestStatus.Cancelled;
            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            TempData["Success"] = "Your request has been cancelled.";
            return RedirectToAction("Dashboard", "Account");
        }

        // POST: Add comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddComment(
            int requestId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Comment cannot be empty.";
                return RedirectToAction(nameof(Details),
                    new { id = requestId });
            }

            var request = await _requestRepo.GetByIdAsync(requestId);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _commentRepo.AddAsync(new RequestComment
            {
                HelpRequestId = requestId,
                AuthorId = userId,
                Content = content.Trim(),
                PostedAt = DateTime.UtcNow
            });
            await _commentRepo.SaveChangesAsync();

            // Notify the other party
            string notifyUserId = userId == request.RequesterId
                ? request.VolunteerId ?? ""
                : request.RequesterId;

            if (!string.IsNullOrEmpty(notifyUserId))
            {
                await _notifRepo.AddAsync(new Notification
                {
                    UserId = notifyUserId,
                    Message = $"New comment on request: {request.Title}",
                    CreatedAt = DateTime.UtcNow,
                    RelatedRequestId = requestId
                });
                await _notifRepo.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details),
                new { id = requestId });
        }

        // GET: Rate volunteer
        [Authorize]
        public async Task<IActionResult> Rate(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (request.RequesterId != userId) return Forbid();

            var volunteer = request.VolunteerId != null
                ? await _userManager.FindByIdAsync(request.VolunteerId)
                : null;

            var model = new RatingViewModel
            {
                HelpRequestId = id,
                VolunteerName = volunteer?.FullName ?? "Volunteer"
            };

            return View(model);
        }

        // POST: Rate volunteer
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Rate(RatingViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (await _ratingRepo.ExistsAsync(
                model.HelpRequestId, userId))
            {
                TempData["Error"] = "You have already rated this volunteer.";
                return RedirectToAction("Dashboard", "Account");
            }

            var request = await _requestRepo
                .GetByIdAsync(model.HelpRequestId);
            if (request?.VolunteerId == null) return NotFound();

            await _ratingRepo.AddAsync(new VolunteerRating
            {
                HelpRequestId = model.HelpRequestId,
                VolunteerId = request.VolunteerId,
                RequesterId = userId,
                Stars = model.Stars,
                Comment = model.Comment,
                RatedAt = DateTime.UtcNow
            });
            await _ratingRepo.SaveChangesAsync();

            TempData["Success"] = "Thank you for your rating!";
            return RedirectToAction("Dashboard", "Account");
        }

        // GET: Save as template
        [Authorize]
        public async Task<IActionResult> SaveTemplate(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (request.RequesterId != userId) return Forbid();

            await _templateRepo.AddAsync(new RequestTemplate
            {
                OwnerId = userId!,
                Title = request.Title,
                Description = request.Description,
                CategoryId = request.CategoryId,
                Urgency = request.Urgency,
                CreatedAt = DateTime.UtcNow
            });
            await _templateRepo.SaveChangesAsync();

            TempData["Success"] =
                "Request saved as a template. You can reuse it from your dashboard.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Delete template
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteTemplate(int templateId)
        {
            await _templateRepo.DeleteAsync(templateId);
            await _templateRepo.SaveChangesAsync();
            TempData["Success"] = "Template deleted.";
            return RedirectToAction("Dashboard", "Account");
        }
    }
}