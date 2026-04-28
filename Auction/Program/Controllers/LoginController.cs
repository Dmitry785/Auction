using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Application.Logic.User;
using Domain.Models;
using Application.Services;

namespace Program.Controllers
{
    [Route("authorization/[action]")]
    public class LoginController(AppDbContext context, LoginService loginService) : Controller
    {
        [HttpGet]
        [Route("/authorization")]
        public IActionResult Index([FromQuery] string? returnUrl)
        {
            return View("Index", returnUrl ?? "");
        }
        [HttpGet]
        public IActionResult Login([FromQuery] string? returnUrl)
        {
            return View("Login", returnUrl ?? string.Empty);
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromQuery]string? returnUrl, [FromForm]LoginRequest loginRequest)
        {
            var userIdResult = await loginService.TryLoginAsync(loginRequest.Username, loginRequest.Password);
            if (userIdResult.Failed)
                return Unauthorized("Wrong password or login");
            var userId = userIdResult.Data!;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginRequest.Username),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Register([FromQuery] string? returnUrl)
        {
            return View("Register", returnUrl ?? string.Empty);
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromQuery] string? returnUrl, [FromForm] RegisterRequest registerRequest)
        {
            var userIdResult = await loginService.TryRegisterAsync(registerRequest.Username, registerRequest.Password, registerRequest.Name);
            if (userIdResult.Failed)
                return Unauthorized("Register failed");
            var userId = userIdResult.Data!;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, registerRequest.Username),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (!Guid.TryParse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return BadRequest();
            if(await context.Users.FirstOrDefaultAsync(x=>x.Id == userId) is null)
                return BadRequest();
            return SignOut();
        }
    }
}
