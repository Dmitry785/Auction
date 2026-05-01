using Application.Common;
using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PaymentService(IAppDbContext context)
    {
        public async Task<Result> PlaceBet(Guid userId, Guid lotId, decimal amount)
        {
            var lot = await context.Lots.Include(x=>x.CurrentBet).ThenInclude(x=>x.BetParticipant).ThenInclude(x=>x.Currencies).FirstOrDefaultAsync(x => x.Id == lotId);
            if (lot is null)
                return Result.Fail("Couldnt find the lot");
            if(amount < (lot.CurrentBet?.BetAmount.Amount ?? 0) + lot.MinBetCurrency.Amount)
                return Result.Fail("Wrong amount");
            var user = await context.Users.Include(x=>x.Currencies).FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return Result.Fail("Couldnt find the user");
            if(lot.CurrentBet?.BetParticipant == user)
                return Result.Fail("You have already placed a bet");
            var wallet = user.Currencies.FirstOrDefault(x => x.Type == lot.MinBetCurrency.Type);
            if (wallet is null)
                return Result.Fail("User havent suitable for the type of currency wallet");

            if (wallet.Amount < amount)
                return Result.Fail("Not enough currency");
            wallet.Amount -= amount;
            if(lot.CurrentBet is not null)
            {
                var cashBackCurrency = lot.CurrentBet.BetParticipant.Currencies.FirstOrDefault(x => x.Type
                == lot.CurrentBet.BetAmount.Type);
                if(cashBackCurrency is not null)
                    cashBackCurrency.Amount += lot.CurrentBet.BetAmount.Amount;
            }
            lot.CurrentBet = new Bet(user, new Money(amount, lot.MinBetCurrency.Type));
            await context.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<Result> WithdrawMoney(Guid userId, Money money)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return Result.Fail("Couldnt find the user");
            var wallet = user.Currencies.FirstOrDefault(x => x.Type == money.Type);
            if (wallet is null)
                return Result.Fail("User havent suitable for the type of currency wallet");
            if (wallet.Amount < money.Amount)
                return Result.Fail("Not enough currency");
            wallet.Amount -= money.Amount;
            await context.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<Result> DepositMoney(Guid userId, Money money)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return Result.Fail("Couldnt find the user");
            var wallet = user.Currencies.FirstOrDefault(x => x.Type == money.Type);
            if (wallet is null)
                return Result.Fail("User havent suitable for the type of currency wallet");
            wallet.Amount += money.Amount;
            await context.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
