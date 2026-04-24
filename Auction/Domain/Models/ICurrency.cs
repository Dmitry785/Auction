using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public interface ICurrency
    {
        decimal Amount { get; set; }
        CurrencyType Type { get; set; }
    }
}
