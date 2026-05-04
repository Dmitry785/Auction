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
using Application.Logic.Item;
using Application.Logic.Lot;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Program.Controllers
{
    [Route("[controller]")]
    public class UserController(IMediator mdtr) : Controller
    {
        [Route("{id:guid}")]
        [HttpGet]
        public async Task<IActionResult> Show([FromRoute] Guid id)
        {
            Console.WriteLine(id.ToString());
            var user = (await mdtr.Send(new GetUserByIdQuery(id))).Data;
            if (user is null)
                return RedirectToAction("Index", "Home");
            return View(new UserViewModel(user.Username, user.Name, user.RegisterDate));
        }
    }
}
