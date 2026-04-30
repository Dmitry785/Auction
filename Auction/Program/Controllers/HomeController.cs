using Application.Logic.Lot;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Program.ViewModels;
using Program.ViewModels.Dto;

namespace Auction.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController(IMediator mdtr) : Controller
    {
        [Route("/")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.SelectedPageName = "Home";
            var lots = (await mdtr.Send(new GetAllLotsQuery()))
                .Select(x => new LotDto(x.StartTime + x.Duration, x.Id, x.ItemInfo.Name,
                x.ItemInfo.Owner.Username, x.ItemInfo.Description, x.ItemInfo.Poster)).ToList();
            return View(new HomeViewModel(lots));
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
