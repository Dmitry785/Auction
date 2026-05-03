using Application.Common;
using Application.Interfaces;
using Domain.Models;
using MediatR;
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
            var lot = await context.Lots
                .Include(x=>x.CurrentBet)
                    .ThenInclude(x=>x.BetParticipant)
                        .ThenInclude(x=>x.Currencies)
                .FirstOrDefaultAsync(x => x.Id == lotId);
            if (lot is null)
                return Result.Fail("Couldnt find the lot");
            if(CheckLotCompleted(lot))
                return Result.Fail("Lot completed");
            if(amount < (lot.CurrentBet?.BetAmount.Amount ?? 0) + lot.MinBetCurrency.Amount)
                return Result.Fail("Wrong amount");
            var user = await context.Users.Include(x=>x.Currencies).FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return Result.Fail("Couldnt find the user");
            if(lot.CurrentBet?.BetParticipant == user)
                return Result.Fail("You have already placed a bet");
            var betMoney = new Money(amount, lot.MinBetCurrency.Type);
            var withdrawOperation = WithdrawMoney(user, betMoney);
            if (withdrawOperation.Failed)
                return withdrawOperation;
            ReturnBetMoney(lot);
            lot.CurrentBet = new Bet(user, betMoney);
            await context.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<Result> BuyoutLot(Guid userId, Guid lotId)
        {
            var lot = await context.Lots
                .Include(x => x.CurrentBet)
                    .ThenInclude(x => x.BetParticipant)
                        .ThenInclude(x => x.Currencies)
                .Include(x=>x.ItemInfo)
                .FirstOrDefaultAsync(x => x.Id == lotId);
            if (lot is null)
                return Result.Fail("Couldnt find the lot");
            if (CheckLotCompleted(lot))
                return Result.Fail("Lot completed");
            if (lot.BuyoutPrice is null)
                return Result.Fail("Lot havent buyout option");
            var user = await context.Users.Include(x => x.Currencies).FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return Result.Fail("Couldnt find the user");
            var withdrawOperation =
                WithdrawMoney(user, lot.BuyoutPrice);
            if (withdrawOperation.Failed)
                return withdrawOperation;
            ReturnBetMoney(lot);
            lot.Completed = true;
            lot.ItemInfo.Owner = user;
            await context.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<Result> WithdrawMoney(Guid userId, Money money)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return Result.Fail("Couldnt find the user");
            var withdrawOperation =
                WithdrawMoney(user, money);
            if (withdrawOperation.Failed)
                return withdrawOperation;
            await context.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<Result> DepositMoney(Guid userId, Money money)
        {
            var user = await context.Users
                .Include(x=>x.Currencies)
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return Result.Fail("Couldnt find the user");
            DepositMoney(user, money);
            await context.SaveChangesAsync();
            return Result.Ok();
        }
        public bool CheckLotCompleted(Guid lotId)
        {
            var lot = context.Lots
            .Include(x => x.ItemInfo)
            .Include(x=>x.LotOwner)
            .Include(x=>x.CurrentBet)
                .ThenInclude(x=>x.BetParticipant)
            .FirstOrDefault(x => x.Id == lotId);
            if (lot is null)
                return true;
            return CheckLotCompleted(lot);
        }
        private bool CheckLotCompleted(Lot lot)
        {
            if (lot.Completed)
                return true;
            if (lot.TimeUntilClosing(DateTime.Now).Ticks < 0)
            {
                lot.Completed = true;
                if (lot.CurrentBet is not null)
                {
                    DepositMoney(lot.LotOwner, lot.CurrentBet.BetAmount);
                    lot.ItemInfo.Owner = lot.CurrentBet.BetParticipant;
                }
                context.SaveChanges();
                return true;
            }
            return false;
        }
        private Result WithdrawMoney(User user, Money money)
        {
            var wallet = user.Currencies.FirstOrDefault(x => x.Type == money.Type);
            if (wallet is null)
                return Result.Fail("User havent suitable for the type of currency wallet");
            if (wallet.Amount < money.Amount)
                return Result.Fail("Not enough currency");
            wallet.Amount -= money.Amount;
            return Result.Ok();
        }
        private void DepositMoney(User user, Money money)
        {
            var wallet = user.Currencies.FirstOrDefault(x => x.Type == money.Type);
            if (wallet is null)
            {
                wallet = new WalletCurrency(0, money.Type);
                user.Currencies.Add(wallet);
            }
            wallet.Amount += money.Amount;
        }
        private void ReturnBetMoney(Lot lot)
        {
            if (lot.CurrentBet is not null)
            {
                DepositMoney(lot.CurrentBet.BetParticipant, lot.CurrentBet.BetAmount);
                lot.CurrentBet = null;
            }
        }
    }
}
