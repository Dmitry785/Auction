using Application.Common;
using Application.Interfaces;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parbad;
using Parbad.AspNetCore;
using Parbad.Gateway.ParbadVirtual;
using Program.ViewModels;

namespace Program.Controllers;
[ApiController]
[Route("[controller]/[action]")]
public class PaymentController(IOnlinePayment onlinePayment, LotsManagementAndPaymentService paymentService, IAppDbContext context):Controller
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> RequestPayment()
    {
        return View(new PaymentRequest(0, "rub"));
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RequestPayment([FromForm]PaymentRequest paymentRequest)
    {
        if(paymentRequest.Amount <= 0)
        {
            ModelState.AddModelError("Validate", "Incorrect amount (must be greater than 0)");
            return View(paymentRequest);
        }
        var userId = HttpContext.User.GetUserId();
        long trackingNumber = Random.Shared.NextInt64();
        decimal amount = paymentRequest.Amount;
        string callbackUrl = Url.Action("VerifyPayment", "Payment", null, Request.Scheme) ?? "/payment/verifypayment";
        CurrencyType currencyType = (CurrencyTypeConverter.TryParse(paymentRequest.AmountType, out var type)) ? (CurrencyType)type! : CurrencyType.RUB;
        var paymentResult = await onlinePayment.RequestAsync(invoice =>
        {
            invoice.SetAmount(amount).SetCallbackUrl(callbackUrl)
            .SetTrackingNumber(trackingNumber).UseParbadVirtual();
        });
        if (paymentResult.IsSucceed)
        {
            var invoice = new Invoice(trackingNumber, userId, amount, currencyType);
            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();
            return paymentResult.GatewayTransporter.TransportToGateway();
        }
        return View(paymentRequest);
    }
    [HttpPost]
    public async Task<IActionResult> VerifyPayment()
    {
        var invoice = await onlinePayment.FetchAsync();
        if (invoice.Status is not PaymentFetchResultStatus.ReadyForVerifying)
            return RedirectToAction("PaymentFailure", new { errorMessage = "This payment already done" });
        var currentInvoice = context.Invoices.FirstOrDefault(x => x.TrackingNumber == invoice.TrackingNumber);
        if (currentInvoice is null || currentInvoice.Status is not PaymentStatus.Pending)
            return RedirectToAction("PaymentFailure", new { errorMessage = "Payment not found or completed" });
        var verifyResult = onlinePayment.Verify(invoice);
        if (verifyResult.IsSucceed)
        {
            currentInvoice.Status = PaymentStatus.Succeeded;
            context.SaveChanges();
            await paymentService.DepositMoney(currentInvoice.UserId, new Domain.Models.Money(currentInvoice.Amount, currentInvoice.AmountType));
            return RedirectToAction("index","currentUser");
        }
        else
        {
            currentInvoice.Status = PaymentStatus.Failed;
            context.SaveChanges();
            return RedirectToAction("PaymentFailure", new { errorMessage = "Payment failed" });
        }
    }
    [HttpGet]
    public IActionResult PaymentFailure(string errorMessage)
    {
        return View(new PaymentFailureViewModel(errorMessage));
    }
}
