using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public static class CurrencyTypeConverter
    {
        public static bool TryParse(string? currency, out CurrencyType? value)
        {
            value = null;
            if (currency is null)
                return false;
            switch (currency.ToLower())
            {
                case "rub":
                    value = CurrencyType.RUB;
                    break;
                case "usd":
                    value = CurrencyType.USD;
                    break;
                case "btc":
                    value = CurrencyType.BTC;
                    break;
                case "eth":
                    value = CurrencyType.ETH;
                    break;
            }
            if(value is not null)
                return true;
            return false;
        }
    }
}
