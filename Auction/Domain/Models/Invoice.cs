using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models;

public enum PaymentStatus
{
    Pending,
    Succeeded,
    Failed
}

public class Invoice : BaseModel<Guid>
{
    public long TrackingNumber { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public CurrencyType AmountType { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Invoice()
    {

    }
    public Invoice(long trackingNumber, Guid userId, decimal amount, CurrencyType type)
    {
        Id = Guid.NewGuid();
        TrackingNumber = trackingNumber;
        UserId = userId;
        Amount = amount;
        AmountType = type;
    }
}