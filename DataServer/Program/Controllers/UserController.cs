using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Program.Controllers
{
    public class UserController(AppDbContext context) : Controller
    {
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var users = context.Users.AsNoTracking().ToList();
            return 
        }
    }
}
