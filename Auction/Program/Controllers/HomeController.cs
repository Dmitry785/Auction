using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Program.ViewModels;

namespace Auction.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        [Route("/")]
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.SelectedPageName = "Home";
            return View();
        }
        [ActionName("About")]
        [HttpGet]
        public IActionResult Index2()
        {
            ViewBag.SelectedPageName = "About";
            return View();
        }
        [HttpGet]
        public IActionResult Contacts()
        {
            ViewBag.SelectedPageName = "Contacts";
            return View();
        }
        [HttpGet]
        [Route("/unauthorized")]
        [ActionName("Unauthorized")]
        public IActionResult UnauthorizedPage()
        {
            return View();
        }
        [HttpGet]
        [Route("/confirm")]
        [ActionName("Confirm")]
        public IActionResult ConfirmPage([FromQuery(Name = "to")]string destinationUrl, [FromQuery]string message)
        {
            return View(new ConfirmViewModel(destinationUrl, message));
        }
    }
}
