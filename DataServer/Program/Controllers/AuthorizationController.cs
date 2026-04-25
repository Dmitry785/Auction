using Microsoft.AspNetCore.Mvc;

namespace Program.Controllers
{
    public class AuthorizationController : Controller
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> Login()
        {
            return Ok();
        }
    }
}
