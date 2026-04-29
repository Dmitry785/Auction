using Application.Logic.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Program.ViewModels;
using System.Security.Claims;

namespace Program.Controllers
{
    [Route("[controller]/[action]")]
    public class UserController(IMediator mdtr) : Controller
    {
        [Authorize]
        [HttpGet]
        [Route("/user")]
        public async Task<IActionResult> Index()
        {
            if (!Guid.TryParse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized("user data error");
            var userResult = await mdtr.Send(new GetUserByIdQuery(userId));
            if (userResult.Failed)
                return Unauthorized("User couldnt be found");
            var user = userResult.Data!;
            ViewBag.SelectedPageName = "Account";
            return View(new UserViewModel(user.Username, user.Name, user.RegisterDate));
        }
        public IActionResult Items()
        {
            return View();
        }
    }
}
