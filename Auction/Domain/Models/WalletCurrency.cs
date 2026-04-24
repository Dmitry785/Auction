using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class WalletCurrency : BaseModel<Guid>, ICurrency
    {
        public decimal Amount { get; set; }
        public CurrencyType Type { get; set; }
        public WalletCurrency(decimal amount, CurrencyType type)
        {
            Amount = amount;
            Type = type;
        }
    }
}
