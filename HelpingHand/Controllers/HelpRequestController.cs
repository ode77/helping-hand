using System.Security.Claims;
using HelpingHand.Models;
using HelpingHand.Repositories;
using HelpingHand.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HelpingHand.Controllers
{
    [Authorize]
    public class HelpRequestController : Controller
    {
        // ── Dependencies ──────────────────────────────────────────────────────
        private readonly IHelpRequestRepository _requestRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IVolunteerApplicationRepository _applicationRepo;
        private readonly INotificationRepository _notifRepo;
        private readonly IRatingRepository _ratingRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly ITemplateRepository _templateRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        // ── Constructor ───────────────────────────────────────────────────────
        public HelpRequestController(
            IHelpRequestRepository requestRepo,
            ICategoryRepository categoryRepo,
            IVolunteerApplicationRepository applicationRepo,
            INotificationRepository notifRepo,
            IRatingRepository ratingRepo,
            ICommentRepository commentRepo,
            ITemplateRepository templateRepo,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _requestRepo = requestRepo;
            _categoryRepo = categoryRepo;
            _applicationRepo = applicationRepo;
            _notifRepo = notifRepo;
            _ratingRepo = ratingRepo;
            _commentRepo = commentRepo;
            _templateRepo = templateRepo;
            _userManager = userManager;
            _env = env;
        }

        // ═════════════════════════════════════════════════════════════════════
        // SECTION 1 — CREATE REQUEST
        // ═════════════════════════════════════════════════════════════════════

        // GET: /HelpRequest/Create
        public async Task<IActionResult> Create(int? templateId)
        {
            var model = new CreateHelpRequestViewModel
            {
                Categories = await _categoryRepo.GetAllAsync()
            };

            // Pre-fill from a saved template if templateId is supplied
            if (templateId.HasValue)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var template = await _templateRepo.GetByIdAsync(templateId.Value);

                if (template != null && template.OwnerId == userId)
                {
                    model.Title = template.Title;
                    model.Description = template.Description;
                    model.CategoryId = template.CategoryId;
                    model.Urgency = template.Urgency;
                    model.TemplateId = templateId;
                    ViewBag.FromTemplate = true;
                }
            }

            return View(model);
        }

        // POST: /HelpRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                Urgency = model.Urgency,
                PreferredDate = model.PreferredDate,
                Status = RequestStatus.Open,
                RequesterId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(14)
            };

            await _requestRepo.AddAsync(request);
            await _requestRepo.SaveChangesAsync();

            TempData["Success"] = "Your request has been posted to the Help Board.";
            return RedirectToAction("Board", "Home");
        }

        // ═════════════════════════════════════════════════════════════════════
        // SECTION 2 — DETAILS
        // ═════════════════════════════════════════════════════════════════════

        // GET: /HelpRequest/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Build the applications list and check if current user already applied
            var applications = request.Applications ?? new List<VolunteerApplication>();
            var alreadyApplied = userId != null &&
                applications.Any(a => a.VolunteerId == userId);

            // Check if already rated
            var alreadyRated = userId != null &&
                await _ratingRepo.ExistsAsync(id, userId);

            // Average rating from the ratings collection
            var ratings = request.Ratings ?? new List<VolunteerRating>();
            var avgRating = ratings.Any() ? ratings.Average(r => r.Stars) : 0.0;
            var ratingCount = ratings.Count();

            var vm = new RequestDetailsViewModel
            {
                Request = request,
                Applications = applications,
                Comments = request.Comments ?? new List<RequestComment>(),
                AlreadyApplied = alreadyApplied,
                AlreadyRated = alreadyRated,
                AverageRating = avgRating,
                RatingCount = ratingCount
            };

            return View(vm);
        }

        // ═════════════════════════════════════════════════════════════════════
        // SECTION 3 — EDIT
        // ═════════════════════════════════════════════════════════════════════

        // GET: /HelpRequest/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (request.RequesterId != userId && !User.IsInRole("Admin"))
                return Forbid();

            if (request.Status != RequestStatus.Open &&
                request.Status != RequestStatus.PendingApproval)
            {
                TempData["Error"] = "Only open or pending requests can be edited.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var model = new CreateHelpRequestViewModel
            {
                Title = request.Title,
                Description = request.Description,
                CategoryId = request.CategoryId,
                Urgency = request.Urgency,
                PreferredDate = request.PreferredDate,
                Categories = await _categoryRepo.GetAllAsync()
            };

            ViewBag.RequestId = id;
            return View(model);
        }

        // POST: /HelpRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateHelpRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryRepo.GetAllAsync();
                ViewBag.RequestId = id;
                return View(model);
            }

            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (request.RequesterId != userId && !User.IsInRole("Admin"))
                return Forbid();

            request.Title = model.Title;
            request.Description = model.Description;
            request.CategoryId = model.CategoryId;
            request.Urgency = model.Urgency;
            request.PreferredDate = model.PreferredDate;

            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            TempData["Success"] = "Request updated.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ═════════════════════════════════════════════════════════════════════
        // SECTION 4 — CANCEL
        // ═════════════════════════════════════════════════════════════════════

        // POST: /HelpRequest/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (request.RequesterId != userId) return Forbid();

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

        // ═════════════════════════════════════════════════════════════════════
        // SECTION 5 — VOLUNTEER APPLICATION (Apply + Accept)
        // ═════════════════════════════════════════════════════════════════════

        // GET: /HelpRequest/Apply/5
        public async Task<IActionResult> Apply(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (request.Status != RequestStatus.Open)
            {
                TempData["Error"] = "This request is no longer accepting applications.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (request.RequesterId == userId)
            {
                TempData["Error"] = "You cannot volunteer for your own request.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Check if user already applied
            var existingApplications = await _applicationRepo.GetByRequestIdAsync(id);
            if (existingApplications.Any(a => a.VolunteerId == userId))
            {
                TempData["Error"] = "You have already applied for this request.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(new VolunteerApplicationViewModel
            {
                HelpRequestId = id,
                RequestTitle = request.Title
            });
        }

        // POST: /HelpRequest/Apply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(VolunteerApplicationViewModel model)
        {
            var request = await _requestRepo.GetByIdAsync(model.HelpRequestId);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Re-check all guards on POST
            if (request.Status != RequestStatus.Open)
            {
                TempData["Error"] = "This request is no longer accepting applications.";
                return RedirectToAction(nameof(Details), new { id = model.HelpRequestId });
            }

            if (request.RequesterId == userId)
            {
                TempData["Error"] = "You cannot volunteer for your own request.";
                return RedirectToAction(nameof(Details), new { id = model.HelpRequestId });
            }

            // ID document is required — validate before ModelState check
            if (model.IdDocument == null || model.IdDocument.Length == 0)
            {
                ModelState.AddModelError("IdDocument", "Please upload a photo of your ID.");
            }
            else
            {
                var allowed = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };
                if (!allowed.Contains(model.IdDocument.ContentType.ToLower()))
                    ModelState.AddModelError("IdDocument",
                        "Only JPEG, PNG, GIF, or PDF files are accepted.");
            }

            if (!ModelState.IsValid)
            {
                model.RequestTitle = request.Title;
                return View(model);
            }

            // ── Save ID document to wwwroot/uploads/ids/ ───────────────────
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "ids");
            Directory.CreateDirectory(uploadsFolder);

            var ext = Path.GetExtension(model.IdDocument!.FileName);
            var fileName = $"{userId}_{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await model.IdDocument.CopyToAsync(stream);

            var idPath = $"/uploads/ids/{fileName}";

            // ── Save application ───────────────────────────────────────────
            var application = new VolunteerApplication
            {
                HelpRequestId = model.HelpRequestId,
                VolunteerId = userId,
                Message = model.Message,
                IdDocumentPath = idPath,
                AppliedAt = DateTime.UtcNow,
                IsAccepted = false,
                IdVerified = false
            };

            await _applicationRepo.AddAsync(application);
            await _applicationRepo.SaveChangesAsync();

            // ── Notify requester ───────────────────────────────────────────
            var volunteer = await _userManager.FindByIdAsync(userId);
            await Notify(
                request.RequesterId,
                $"{volunteer?.FullName ?? "A volunteer"} has applied for your request: \"{request.Title}\".",
                request.HelpRequestId);

            TempData["Success"] = "Application submitted! The requester will review it shortly.";
            return RedirectToAction(nameof(Details), new { id = model.HelpRequestId });
        }

        // POST: /HelpRequest/AcceptApplication
        // Requester selects a volunteer → PendingApproval (admin must verify ID)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptApplication(int applicationId)
        {
            var application = await _applicationRepo.GetByIdAsync(applicationId);
            if (application == null) return NotFound();

            var request = await _requestRepo.GetByIdAsync(application.HelpRequestId);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (request.RequesterId != userId) return Forbid();

            if (request.Status != RequestStatus.Open)
            {
                TempData["Error"] = "This request is no longer open.";
                return RedirectToAction(nameof(Details), new { id = request.HelpRequestId });
            }

            // Mark accepted on the application record
            application.IsAccepted = true;
            request.Status = RequestStatus.PendingApproval;
            request.VolunteerId = application.VolunteerId;

            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            var requester = await _userManager.FindByIdAsync(userId);
            await Notify(
                application.VolunteerId,
                $"{requester?.FullName ?? "The requester"} accepted your application for \"{request.Title}\". Awaiting admin approval.",
                request.HelpRequestId);

            TempData["Success"] = "Application accepted. An admin will verify the ID before assigning.";
            return RedirectToAction(nameof(Details), new { id = request.HelpRequestId });
        }

        // ═════════════════════════════════════════════════════════════════════
        // SECTION 6 — DUAL CONFIRMATION
        // ═════════════════════════════════════════════════════════════════════

        // POST: /HelpRequest/VolunteerMarkDone/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VolunteerMarkDone(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (request.Status != RequestStatus.Assigned)
            {
                TempData["Error"] = "This request is not in an assigned state.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (request.VolunteerId != userId) return Forbid();

            request.Status = RequestStatus.VolunteerDone;
            request.VolunteerConfirmedDone = true;

            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            var volunteer = await _userManager.FindByIdAsync(userId);
            await Notify(
                request.RequesterId,
                $"{volunteer?.FullName ?? "Your volunteer"} marked \"{request.Title}\" as done. Please confirm to complete the request.",
                request.HelpRequestId);

            TempData["Success"] = "Done! The requester has been notified to confirm.";
            return RedirectToAction("Dashboard", "Account");
        }

        // GET: /HelpRequest/RequesterConfirm/5
        public async Task<IActionResult> RequesterConfirm(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (request.RequesterId != userId) return Forbid();

            if (request.Status != RequestStatus.VolunteerDone)
            {
                TempData["Error"] = "This request cannot be confirmed at this stage.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var volunteer = request.Volunteer != null
                ? request.Volunteer
                : await _userManager.FindByIdAsync(request.VolunteerId ?? "");

            return View(new RequesterConfirmViewModel
            {
                HelpRequestId = id,
                RequestTitle = request.Title,
                VolunteerName = volunteer?.FullName ?? "the volunteer"
            });
        }

        // POST: /HelpRequest/RequesterConfirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequesterConfirm(RequesterConfirmViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var request = await _requestRepo.GetByIdAsync(model.HelpRequestId);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (request.RequesterId != userId) return Forbid();

            if (request.Status != RequestStatus.VolunteerDone)
            {
                TempData["Error"] = "This request cannot be confirmed at this stage.";
                return RedirectToAction(nameof(Details), new { id = model.HelpRequestId });
            }

            // ── Complete the request ───────────────────────────────────────
            request.Status = RequestStatus.Completed;
            request.RequesterConfirmedDone = true;
            request.RequesterFeedback = model.Feedback;

            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            // ── Save the star rating ──────────────────────────────────────
            if (!string.IsNullOrEmpty(request.VolunteerId))
            {
                var rating = new VolunteerRating
                {
                    HelpRequestId = model.HelpRequestId,
                    VolunteerId = request.VolunteerId,
                    RequesterId = userId,
                    Stars = model.Stars,
                    Comment = model.Feedback,
                    RatedAt = DateTime.UtcNow
                };
                await _ratingRepo.AddAsync(rating);
                await _ratingRepo.SaveChangesAsync();

                // ── Update volunteer badge count ───────────────────────────
                var volunteer = await _userManager.FindByIdAsync(request.VolunteerId);
                if (volunteer != null)
                {
                    volunteer.CompletedHelpCount++;
                    await _userManager.UpdateAsync(volunteer);
                }

                // ── Notify volunteer ───────────────────────────────────────
                var requester = await _userManager.FindByIdAsync(userId);
                await Notify(
                    request.VolunteerId,
                    $"{requester?.FullName ?? "The requester"} confirmed \"{request.Title}\" as complete and gave you {model.Stars} star{(model.Stars != 1 ? "s" : "")}.",
                    request.HelpRequestId);
            }

            TempData["Success"] = "Thank you! The request is now marked as complete.";
            return RedirectToAction("Dashboard", "Account");
        }

        // ═════════════════════════════════════════════════════════════════════
        // SECTION 7 — COMMENTS
        // ═════════════════════════════════════════════════════════════════════

        // POST: /HelpRequest/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int helpRequestId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Comment cannot be empty.";
                return RedirectToAction(nameof(Details), new { id = helpRequestId });
            }

            var request = await _requestRepo.GetByIdAsync(helpRequestId);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Only the requester and assigned volunteer may comment
            if (request.RequesterId != userId && request.VolunteerId != userId)
            {
                TempData["Error"] = "Only the requester and volunteer can leave comments.";
                return RedirectToAction(nameof(Details), new { id = helpRequestId });
            }

            var comment = new RequestComment
            {
                HelpRequestId = helpRequestId,
                AuthorId = userId,
                Content = content.Trim(),
                PostedAt = DateTime.UtcNow
            };

            await _commentRepo.AddAsync(comment);
            await _commentRepo.SaveChangesAsync();

            // Notify the other party
            var recipientId = request.RequesterId == userId
                ? request.VolunteerId
                : request.RequesterId;

            if (!string.IsNullOrEmpty(recipientId))
            {
                var author = await _userManager.FindByIdAsync(userId);
                await Notify(
                    recipientId,
                    $"{author?.FullName ?? "Someone"} left a comment on \"{request.Title}\".",
                    helpRequestId);
            }

            return RedirectToAction(nameof(Details), new { id = helpRequestId });
        }

        // ═════════════════════════════════════════════════════════════════════
        // SECTION 8 — TEMPLATES
        // ═════════════════════════════════════════════════════════════════════

        // POST: /HelpRequest/SaveAsTemplate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAsTemplate(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (request.RequesterId != userId) return Forbid();

            var template = new RequestTemplate
            {
                OwnerId = userId,
                Title = request.Title,
                Description = request.Description,
                CategoryId = request.CategoryId,
                Urgency = request.Urgency,
                CreatedAt = DateTime.UtcNow
            };

            await _templateRepo.AddAsync(template);
            await _templateRepo.SaveChangesAsync();

            TempData["Success"] = "Saved as template. You can reuse it from your Dashboard.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /HelpRequest/DeleteTemplate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTemplate(int templateId)
        {
            var template = await _templateRepo.GetByIdAsync(templateId);
            if (template == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (template.OwnerId != userId) return Forbid();

            await _templateRepo.DeleteAsync(templateId);
            await _templateRepo.SaveChangesAsync();

            TempData["Success"] = "Template deleted.";
            return RedirectToAction("Dashboard", "Account");
        }

        // ═════════════════════════════════════════════════════════════════════
        // SECTION 9 — ADMIN ACTIONS (approve / reject / verify ID / assign)
        // These act on HelpRequest lifecycle so they live here, gated by Admin role
        // ═════════════════════════════════════════════════════════════════════

        // POST: /HelpRequest/AdminVerifyId
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminVerifyId(int applicationId)
        {
            var application = await _applicationRepo.GetByIdAsync(applicationId);
            if (application == null) return NotFound();

            application.IdVerified = true;
            await _applicationRepo.SaveChangesAsync();

            TempData["Success"] = "ID verified. You can now approve the request.";
            return RedirectToAction("Index", "Admin");
        }

        // POST: /HelpRequest/AdminApprove/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminApprove(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            if (request.Status != RequestStatus.PendingApproval)
            {
                TempData["Error"] = "Only pending approval requests can be approved.";
                return RedirectToAction("Index", "Admin");
            }

            request.Status = RequestStatus.Assigned;
            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            if (!string.IsNullOrEmpty(request.VolunteerId))
            {
                await Notify(
                    request.VolunteerId,
                    $"Your application for \"{request.Title}\" has been approved. You can now view the requester's contact details.",
                    request.HelpRequestId);
            }

            TempData["Success"] = "Request approved and volunteer assigned.";
            return RedirectToAction("Index", "Admin");
        }

        // POST: /HelpRequest/AdminReject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminReject(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            if (request.Status != RequestStatus.PendingApproval)
            {
                TempData["Error"] = "Only pending approval requests can be rejected.";
                return RedirectToAction("Index", "Admin");
            }

            var rejectedVolunteerId = request.VolunteerId;
            request.Status = RequestStatus.Open;
            request.VolunteerId = null;

            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            if (!string.IsNullOrEmpty(rejectedVolunteerId))
            {
                await Notify(
                    rejectedVolunteerId,
                    $"Your application for \"{request.Title}\" was not approved. The request is now open for other volunteers.",
                    request.HelpRequestId);
            }

            TempData["Success"] = "Application rejected. Request returned to Open.";
            return RedirectToAction("Index", "Admin");
        }

        // POST: /HelpRequest/AdminManualAssign
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminManualAssign(int id, string volunteerId)
        {
            if (string.IsNullOrWhiteSpace(volunteerId))
            {
                TempData["Error"] = "Please select a volunteer.";
                return RedirectToAction("Index", "Admin");
            }

            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            var volunteer = await _userManager.FindByIdAsync(volunteerId);
            if (volunteer == null)
            {
                TempData["Error"] = "Selected user not found.";
                return RedirectToAction("Index", "Admin");
            }

            if (request.RequesterId == volunteerId)
            {
                TempData["Error"] = "Cannot assign the requester as the volunteer.";
                return RedirectToAction("Index", "Admin");
            }

            request.Status = RequestStatus.Assigned;
            request.VolunteerId = volunteerId;

            await _requestRepo.UpdateAsync(request);
            await _requestRepo.SaveChangesAsync();

            await Notify(
                volunteerId,
                $"An admin has assigned you to: \"{request.Title}\". Check the request details for contact information.",
                request.HelpRequestId);

            TempData["Success"] = $"{volunteer.FullName} has been manually assigned.";
            return RedirectToAction("Index", "Admin");
        }

        // ═════════════════════════════════════════════════════════════════════
        // PRIVATE HELPER — send a notification in one line
        // ═════════════════════════════════════════════════════════════════════
        private async Task Notify(string userId, string message, int relatedRequestId)
        {
            await _notifRepo.AddAsync(new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                RelatedRequestId = relatedRequestId
            });
            await _notifRepo.SaveChangesAsync();
        }
    }
}