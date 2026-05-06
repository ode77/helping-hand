using HelpingHand.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HelpingHand.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHelpRequestRepository _requestRepo;

        public HomeController(IHelpRequestRepository requestRepo)
            => _requestRepo = requestRepo;

        public IActionResult Index() => View();

        public async Task<IActionResult> Board()
        {
            var requests = await _requestRepo.GetOpenRequestsAsync();
            return View(requests);
        }
    }
}