using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Program
{
    public sealed record LoginGameRequest([Required] string Username, [Required] string Password);
    public sealed record LoginRequest([Required] string Username, [Required] string Password);
    public sealed record RegisterRequest(
        [Required]
        string Username,
        [Required]
        string Password,
        string Name);
    public sealed record PaymentRequest(decimal Amount,
        [CurrencyTypeValidation(ErrorMessage = "rub, usd, btc, eth allowed")]
        string AmountType);
    public sealed record CreateLotRequest([Required] TimeOnly Duration, [Required]decimal MinBetAmount, [Required][CurrencyTypeValidation] string MinBetCurrencyType
        , decimal? BuyoutAmount, [CurrencyTypeValidation]string? BuyoutCurrencyType);

    public class CurrencyTypeValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? valueObj)
        {
            if (valueObj is null)
                return true;
            if(valueObj is string value)
            {
                value = value.ToLower();
                return value == "rub" || value == "usd" || value == "btc" || value == "eth";
            }
            return false;
        }
    }

}

