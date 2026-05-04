using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ArchivalLot : BaseModel<Guid>
    {
        public Item ItemInfo { get; set; } = null!;
        public User LotOwner { get; set; } = null!;
        public LotArchiveEndType EndType { get; set; }
        public Money? BoughtFor { get; set; }
        public User? Buyer { get; set; }
        public DateTime CompletionTime { get; set; }
        public ArchivalLot(Item itemInfo, User lotOwner, Money? boughtFor, User? buyer, LotArchiveEndType endType, DateTime completionTime)
        {
            Id = Guid.NewGuid();
            ItemInfo = itemInfo;
            Buyer = buyer;
            LotOwner = lotOwner;
            EndType = endType;
            BoughtFor = boughtFor;
            CompletionTime = completionTime;
        }
        public ArchivalLot()
        {
        }
    }
    public enum LotArchiveEndType
    {
        Bought,
        Expired,
        Canceled
    }
}
