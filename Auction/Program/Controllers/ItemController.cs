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
using Application.Common;

namespace Program.Controllers
{
    [Route("[controller]")]
    public class ItemController(IMediator mdtr, LotsManagementAndPaymentService paymentService) : Controller
    {
        [Route("{id}")]
        [HttpGet]
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
        [Route("{id}/createLot")]
        [HttpGet]
        [Authorize(Policy = "LinkedToTheOriginalAccount")]
        public IActionResult CreateLot([FromRoute]string id)
        {
            ViewBag.ItemId = id;
            return View(new CreateLotRequest(TimeOnly.MinValue, 0, string.Empty, null, null));
        }
        [Route("{id}/createLot")]
        [HttpPost]
        [Authorize(Policy = "LinkedToTheOriginalAccount")]
        public async Task<IActionResult> CreateLot([FromRoute] string id, [FromForm]CreateLotRequest data)
        {
            if (data.MinBetAmount == 0)
                ModelState.AddModelError("Validate", "Min bet amount must be greater than 0");
            if (data.Duration.Minute == 0)
                ModelState.AddModelError("Validate", "Duration must be greater than 1 min");
            if(!CurrencyTypeConverter.TryParse(data.MinBetCurrencyType, out var minBetCurrency))
                ModelState.AddModelError("Validate", "Duration must be greater than 1 min");
            if (!ModelState.IsValid)
                return View(data);

            var lotResult = await paymentService.CreateNewLot(data.Duration, id,
                new Money(data.MinBetAmount, (CurrencyType)minBetCurrency!),
                data.BuyoutCurrencyType is not null && data.BuyoutAmount is not null
                && CurrencyTypeConverter.TryParse(data.BuyoutCurrencyType, out var buyoutCurrency)
                ? new Money((decimal)data.BuyoutAmount!, (CurrencyType)buyoutCurrency!) : null);
            if (lotResult.Failed)
            {
                TempData["ErrorMessage"] = lotResult.ErrorMessage;
                return View(data);
            }
            return RedirectToAction("Show", "Lot", new { Id = lotResult.Data! });
        }
    }
}
