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
    public class ItemController(IMediator mdtr) : Controller
    {
        [Route("{id}")]
        public async Task<IActionResult> Show([FromRoute] string id)
        {
            Console.WriteLine(id.ToString());
            var item = (await mdtr.Send(new GetItemByIdQuery(id))).Data;
            if (item is null)
                return RedirectToAction("Index", "Home");
            var lot = (await mdtr.Send(new GetAllLotsQuery(x => x.ItemInfo.Id == item.Id))).FirstOrDefault();
            return View(new ItemViewModel(new ViewModels.Dto.ItemDto(item.Name, item.Description, 
                lot is null?null:Url.Action("show", "lot", new { id = lot.Id}), 
                item.Poster, item.Owner.Username, item.Type.ToString())));
        }
    }
}
