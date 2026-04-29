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
using Program.ViewModels;
using Microsoft.AspNetCore.Identity.Data;

namespace Program.Controllers
{
    [Route("authorization/[action]")]
    public class LoginController(LoginService loginService) : Controller
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
            ViewBag.ReturnUrl = returnUrl ?? "/";
            return View(new LoginRequest(string.Empty, string.Empty));
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromQuery]string? returnUrl, [FromForm]LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
                return View(loginRequest);
            var userIdResult = await loginService.TryLoginAsync(loginRequest.Username, loginRequest.Password);
            if (userIdResult.Failed)
            {
                ModelState.AddModelError("Validate", "Wrong password or login");
                return View(loginRequest);
            }
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
            ViewBag.ReturnUrl = returnUrl ?? "/";
            return View(new RegisterRequest(string.Empty, string.Empty, string.Empty));
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromQuery] string? returnUrl, [FromForm] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
                return View(registerRequest);
            var userIdResult = await loginService.TryRegisterAsync(registerRequest.Username, registerRequest.Password, registerRequest.Name);
            if (userIdResult.Failed)
            {
                ModelState.AddModelError("Validate", "Register failed");
                return View(registerRequest);
            }
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
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
