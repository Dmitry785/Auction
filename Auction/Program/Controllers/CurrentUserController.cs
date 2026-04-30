using Application.Logic.User;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Program.ViewModels;
using System.Security.Claims;
using Application.Logic.Item;
using Program.ViewModels.Dto;
using Application.Logic.Lot;

namespace Program.Controllers
{
    [Route("[controller]/[action]")]
    public class CurrentUserController(IMediator mdtr, LoginService loginService, DataServerApiService apiService) : Controller
    {
        [Authorize]
        [HttpGet]
        [Route("/user")]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized("user data error");
            var userResult = await mdtr.Send(new GetUserByIdQuery(userId));
            if (userResult.Failed)
                return RedirectToAction("logout", "login");
            var user = userResult.Data!;
            ViewBag.SelectedPageName = "Account";
            return View(new UserViewModel(user.Username, user.Name, user.RegisterDate));
        }
        [Authorize(Policy = "LinkedToTheOriginalAccount")]
        [HttpGet]
        public async Task<IActionResult> Items()
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized("user data error");
            var userResult = await mdtr.Send(new GetUserByIdQuery(userId));
            if (userResult.Failed)
                return RedirectToAction("logout", "login");
            var user = userResult.Data!;
            ViewBag.SelectedPageName = "Account";
            var items = (await mdtr.Send(new GetAllItemsQuery(x => x.Owner == user)))
                .Select(x=> {
                    var lot = (mdtr.Send(new GetAllLotsQuery(l => l.ItemInfo.Id == x.Id)).Result).FirstOrDefault();
                    return new ItemDto(x.Name, x.Description, lot is null ? null : Url.Action("Show", "Lot", new { id = lot.Id }),
                        x.Poster, x.Owner.Username, x.Type.ToString());
                    }
                );
            return View(new ItemsViewModel(items.ToList()));
        }
        [Authorize]
        [HttpGet]
        public IActionResult LinkGameAccount()
        {
            ViewBag.SelectedPageName = "Account";
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LinkGameAccount(LoginGameRequest loginRequest)
        {
            Guid userId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            string username = HttpContext.User.FindFirstValue(ClaimTypes.Name)!;
            if (!ModelState.IsValid)
                return View(loginRequest);
            var userGameIdResult = await apiService.RequestUserId(loginRequest.Username, loginRequest.Password);
            if (userGameIdResult.Failed)
            {
                ModelState.AddModelError("Validate", "Wrong password or login");
                return View(loginRequest);
            }
            var userGameId = userGameIdResult.Data!;
            await loginService.LinkUser(userId, userGameId);
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("linked_account_id", userGameId.ToString())
        };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties { IsPersistent = true };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);
            return RedirectToAction("Index", "User");
        }
    }
}
