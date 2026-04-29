using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Program
{
    public sealed record LoginRequest([Required] string Username, [Required] string Password);
    public sealed record RegisterRequest(
        [Required]
        [MinLength(3, ErrorMessage = "Username length must be more than 3 chars")]
        string Username, 
        [Required] 
        string Password, 
        string Name);
}
