using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Lot : BaseModel<Guid>
    {
        public Item ItemInfo { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public Money? BuyoutPrice { get; set; }
        public Bet? CurrentBet { get; set; }
        public Money MinBetCurrency { get; set; } = null!;
        public TimeSpan TimeUntilClosing(DateTime currentTime)
        {
            return StartTime + Duration - currentTime;
        }
        public Lot(Item itemInfo, DateTime startTime, TimeSpan duration, Money minBetCurrency, Money? buyoutPrice = null)
        {
            Id = Guid.NewGuid();
            ItemInfo = itemInfo;
            StartTime = startTime;
            Duration = duration;
            BuyoutPrice = buyoutPrice;
            MinBetCurrency = minBetCurrency;
        }
        public Lot()
        {
        }
    }
}
