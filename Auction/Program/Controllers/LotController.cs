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

namespace Program.Controllers
{
    [Route("[controller]")]
    public class LotController(IMediator mdtr) : Controller
    {
        [Route("{id:guid}")]
        public async Task<IActionResult> Show([FromRoute]Guid id)
        {
            Console.WriteLine(id.ToString());
            var lot = (await mdtr.Send(new GetLotByIdQuery(id))).Data;
            if (lot is null)
                return RedirectToAction("Index", "Home");
            return View(new LotViewModel(lot.ItemInfo.Name, lot.ItemInfo.Description,
                lot.ItemInfo.Poster, lot.ItemInfo.Type, lot.ItemInfo.Owner.Name, lot.BuyoutPrice, lot.MinBetCurrency, 
                lot.CurrentBet?.BetAmount, lot.CurrentBet?.BetParticipant.Name));
        }
    }
}
