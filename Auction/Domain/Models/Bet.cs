using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [Owned]
    public class Bet
    {
        public User BetParticipant { get; set; } = null!;
        public Money BetAmount { get; set; } = null!;
        public Bet(User betParticipant, Money betAmount)
        {
            BetParticipant = betParticipant;
            BetAmount = betAmount;
        }
        public Bet()
        {

        }
    }
}
