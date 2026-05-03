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
    [Route("[controller]/{id:guid}")]
    public class LotController(IMediator mdtr, PaymentService paymentService) : Controller
    {
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Show([FromRoute] Guid id)
        {
            var lot = (await mdtr.Send(new GetLotByIdQuery(id))).Data;
            if (lot is null)
                return RedirectToAction("Index", "Home");
            if (paymentService.CheckLotCompleted(id))
            {
                lot = (await mdtr.Send(new GetLotByIdQuery(id))).Data;
                if (lot is null)
                    return RedirectToAction("Index", "Home");
                return View(new LotViewModel(lot.Id, lot.ItemInfo.Id, lot.LotOwner.Id, lot.ItemInfo.Name, lot.ItemInfo.Description,
                    lot.ItemInfo.Poster, lot.ItemInfo.Type, lot.LotOwner.Name, lot.BuyoutPrice, lot.MinBetCurrency,
                    lot.CurrentBet?.BetAmount, lot.CurrentBet?.BetParticipant.Username, lot.StartTime + lot.Duration,
                    false, false, false, false, null, null, true));
            }
            var userId = HttpContext.User.GetUserId();
            var hasUserLinkedAccount = HttpContext.User.GetLinkedAccountId() != null;
            await Console.Out.WriteLineAsync(userId.ToString());
            bool isAuthorized = false;
            bool canUserBuyout = false;
            bool canUserBet = false;
            decimal? minBet = null;
            WalletCurrency? betCurrency = null;
            if (userId != Guid.Empty) {
                isAuthorized = true;
                var user = (await mdtr.Send(new GetUserByIdQuery(userId))).Data;
                if (user is not null && hasUserLinkedAccount)
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
            return View(new LotViewModel(lot.Id, lot.ItemInfo.Id, lot.LotOwner.Id, lot.ItemInfo.Name, lot.ItemInfo.Description,
                lot.ItemInfo.Poster, lot.ItemInfo.Type, lot.LotOwner.Name, lot.BuyoutPrice, lot.MinBetCurrency,
                lot.CurrentBet?.BetAmount, lot.CurrentBet?.BetParticipant.Username, lot.StartTime + lot.Duration,
                isAuthorized, hasUserLinkedAccount, canUserBet, canUserBuyout, minBet, betCurrency?.Amount, false));
        }
        [Route("buyout")]
        [Authorize(Policy = "LinkedToTheOriginalAccount")]
        [HttpGet]
        public async Task<IActionResult> Buyout([FromRoute] Guid id)
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == Guid.Empty)
                return RedirectToAction("Index", "Login");
            var paymentResult = await paymentService.BuyoutLot(userId, id);
            if (paymentResult.Failed)
            {
                TempData["ErrorMessage"] = paymentResult.ErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Items", "CurrentUser");
        }
        [Route("bet")]
        [Authorize(Policy = "LinkedToTheOriginalAccount")]
        [HttpPost]
        public async Task<IActionResult> Bet([FromRoute] Guid id, [FromForm(Name = "amount")]string amountString)
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == Guid.Empty)
                return BadRequest();
            if (!decimal.TryParse(amountString, out var amount))
                return BadRequest();
            var paymentResult = await paymentService.PlaceBet(userId, id, amount);
            if (paymentResult.Failed)
            {
                TempData["ErrorMessage"] = paymentResult.ErrorMessage;
                return BadRequest();
            }
            return Ok();
        }
    }
}
