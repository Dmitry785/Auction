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
            var userId = HttpContext.User.GetUserId();
            await Console.Out.WriteLineAsync(userId.ToString());
            bool isAuthorized = false;
            bool canUserBuyout = false;
            bool canUserBet = false;
            decimal? minBet = null;
            WalletCurrency? betCurrency = null;
            if (userId != Guid.Empty) {
                isAuthorized = true;
                var user = (await mdtr.Send(new GetUserByIdQuery(userId))).Data;
                if (user is not null)
                {
                    betCurrency = user.Currencies.FirstOrDefault(x => x.Type == lot.MinBetCurrency.Type);
                    minBet = (lot.CurrentBet?.BetAmount.Amount ?? 0) + lot.MinBetCurrency.Amount;
                    if (betCurrency is not null && betCurrency.Amount >= minBet)
                    {
                        canUserBet = true;
                    }
                    if(lot.BuyoutPrice is not null)
                    {
                        var buyoutCurrency = user.Currencies.FirstOrDefault(x => x.Type == lot.BuyoutPrice.Type);
                        if(buyoutCurrency is not null && buyoutCurrency.Amount >= lot.BuyoutPrice.Amount)
                        {
                            canUserBuyout = true;
                        }
                    }
                }
            }
            return View(new LotViewModel(lot.ItemInfo.Name, lot.ItemInfo.Description,
                lot.ItemInfo.Poster, lot.ItemInfo.Type, lot.ItemInfo.Owner.Name, lot.BuyoutPrice, lot.MinBetCurrency,
                lot.CurrentBet?.BetAmount, lot.CurrentBet?.BetParticipant.Name, lot.StartTime + lot.Duration,
                isAuthorized, canUserBet, canUserBuyout, minBet, betCurrency?.Amount));
        }
        [Route("{id:guid}/buyout")]
        [Authorize(Policy = "LinkedToTheOriginalAccount")]
        public async Task<IActionResult> Buyout([FromRoute] Guid id)
        {
            var lot = (await mdtr.Send(new GetLotByIdQuery(id))).Data;
            if (lot is null || lot.BuyoutPrice is null)
                return RedirectToAction("Index", "Home");
            var userId = HttpContext.User.GetUserId();
            if (userId == Guid.Empty)
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
