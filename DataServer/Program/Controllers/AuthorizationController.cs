using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using Infrastructure;
using Program.Requests;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Program.Controllers
{
    [ApiController]
    public class AuthorizationController(AppDbContext context) : Controller
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] string? returnUrl, LoginRequest loginRequest)
        {
            var user = context.Users.FirstOrDefault(x => x.Username == loginRequest.Username && 
            x.PasswordHash == loginRequest.Password.GetHashCode());
            if (user is null) 
                return Unauthorized();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
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
}
