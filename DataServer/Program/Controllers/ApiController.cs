using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using Infrastructure;
using Program.Requests;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Application.Services;
using Domain.Models;

namespace Program.Controllers
{
    [ApiController]
    public class ApiController(CommonService service) : Controller
    {
        [HttpPost]
        [Route("userId")]
        public async Task<IActionResult> GetUserId([FromQuery] string? returnUrl, LoginRequest loginRequest)
        {
            var userId = service.GetUserId(loginRequest.Username, loginRequest.Password);
            if (userId.Failed)
                return NotFound();
            return Ok(userId.Data!);
        }
        [HttpPost]
        [Route("userItems")]
        public async Task<IActionResult> GetUserItems([FromQuery] string? returnUrl, [FromBody] Guid userId)
        {
            var items = service.GetAllUserItems(userId);
            if (items.Failed)
                return NotFound();
            return Ok(items.Data!);
        }
        [HttpPost]
        [Route("hold")]
        public async Task<IActionResult> HoldItem([FromQuery] string? returnUrl, [FromBody] Guid itemId)
        {
            var result = service.HoldItem(itemId);
            if (result.Failed)
                return NotFound();
            return Ok();
        }
        [HttpPost]
        [Route("move")]
        public async Task<IActionResult> MoveItem([FromQuery] string? returnUrl, [FromBody] Guid itemId, Guid newOwnerId)
        {
            service.UnholdItem(itemId);
            var result = service.MoveItem(itemId, newOwnerId);
            if (result.Failed)
                return NotFound();
            return Ok();
        }
    }
}
