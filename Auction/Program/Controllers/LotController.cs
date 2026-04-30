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
    public class LotController(IMediator mdtr, PaymentService paymentService) : Controller
    {
        [Route("{id:guid}")]
        public async Task<IActionResult> Show([FromRoute] Guid id)
        {
            Console.WriteLine(id.ToString());
            var lot = (await mdtr.Send(new GetLotByIdQuery(id))).Data;
            if (lot is null)
                return RedirectToAction("Index", "Home");
            return View(new LotViewModel(lot.ItemInfo.Name, lot.ItemInfo.Description,
                lot.ItemInfo.Poster, lot.ItemInfo.Type, lot.ItemInfo.Owner.Name, lot.BuyoutPrice, lot.MinBetCurrency,
                lot.CurrentBet?.BetAmount, lot.CurrentBet?.BetParticipant.Name, lot.StartTime + lot.Duration));
        }
        [Route("{id:guid}/buyout")]
        [Authorize(Policy = "LinkedToTheOriginalAccount")]
        public async Task<IActionResult> Buyout([FromRoute] Guid id)
        {
            var lot = (await mdtr.Send(new GetLotByIdQuery(id))).Data;
            if (lot is null || lot.BuyoutPrice is null)
                return RedirectToAction("Index", "Home");
            if(!Guid.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
                return RedirectToAction("Index", "Login");
            var paymentResult = await paymentService.WithdrawMoney(userId, lot.BuyoutPrice);
            if (paymentResult.Failed)
            {
                TempData["ErrorMessage"] = paymentResult.ErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Items", "User");
        }
    }
}
