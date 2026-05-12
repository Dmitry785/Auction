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
        [HttpPost("users")]
        public IActionResult GetAllUsers()
        {
            return Ok(service.GetAllUsers().Select(x=>new UserData(x.Id.ToString(), x.Name, x.Username)));
        }
        [HttpPost("items")]
        public IActionResult GetAllItems()
        {
            return Ok(service.GetAllItems().Select(x => new ItemData(x.Id.ToString(), x.Name, x.Description, x.Type, x.Owner.Id.ToString(), x.Poster)));
        }
        [HttpPost("userid")]
        public IActionResult GetUserId([FromBody]LoginRequest loginRequest)
        {
            var userId = service.GetUserId(loginRequest.Username, loginRequest.Password);
            if (userId.Failed)
                return NotFound();
            return Ok(userId.Data!);
        }
        [HttpPost("userItems")]
        public IActionResult GetUserItems([FromBody] Guid id)
        {
            var items = service.GetAllUserItems(id);
            if (items.Failed)
                return NotFound();
            return Ok(items.Data!
                .Select(x => new ItemData(x.Id.ToString(), x.Name, 
                x.Description, x.Type, x.Owner.Id.ToString(), x.Poster)));
        }
        [HttpPost("hold")]
        public IActionResult HoldItem([FromBody] Guid itemId)
        {
            var result = service.HoldItem(itemId);
            if (result.Failed)
                return NotFound();
            return Ok();
        }
        [HttpPost("unhold")]
        public IActionResult UnholdItem([FromBody] Guid itemId)
        {
            var result = service.UnholdItem(itemId);
            if (result.Failed)
                return NotFound();
            return Ok();
        }
        [HttpPost("move")]
        public IActionResult MoveItem([FromBody] MoveItemRequest request)
        {
            service.UnholdItem(request.ItemId);
            var result = service.MoveItem(request.ItemId, request.NewOwnerId);
            if (result.Failed)
                return NotFound();
            return Ok();
        }
        [HttpPost("addItem")]
        public IActionResult AddItem([FromBody] AddItemRequest itemInfo)
        {
            var res = service.AddNewItem(itemInfo.Name, itemInfo.Description, itemInfo.ItemType, itemInfo.OwnerId, itemInfo.Poster);
            if (res.Failed)
                return BadRequest();
            return Ok(res.Data!);
        }
        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] RegisterRequest regInfo)
        {
            var res = service.RegisterUser(regInfo.Username, regInfo.Password, regInfo.Name);
            if (res.Failed)
                return BadRequest();
            return Ok(res.Data!);
        }
    }
}
