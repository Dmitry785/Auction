using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [Owned]
    public class Money : ICurrency
    {
        public decimal Amount { get; set; }
        public CurrencyType Type { get; set; }
        public Money(decimal amount, CurrencyType type = CurrencyType.RUB)
        {
            Amount = amount;
            Type = type;
        }
    }
    public enum CurrencyType
    {
        BTC,
        ETH,
        USD,
        RUB
    }
}
