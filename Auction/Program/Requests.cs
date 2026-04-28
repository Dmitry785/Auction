using Microsoft.AspNetCore.Mvc;

namespace Program
{
    public sealed record LoginRequest([FromForm(Name = "login")] string Username, string Password);
    public sealed record RegisterRequest([FromForm(Name = "login")]string Username, string Password, string Name);
}
