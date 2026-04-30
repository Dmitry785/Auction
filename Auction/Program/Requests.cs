using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Program
{
    public sealed record LoginGameRequest([Required] string Username, [Required] string Password);
    public sealed record LoginRequest([Required] string Username, [Required] string Password);
    public sealed record RegisterRequest(
        [Required]
        string Username, 
        [Required] 
        string Password, 
        string Name);
}
