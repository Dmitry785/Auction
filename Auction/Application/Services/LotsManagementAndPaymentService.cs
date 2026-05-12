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
    public class LotsManagementAndPaymentService(IAppDbContext context, DataServerApiService apiService)
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
            if(await CheckLotCompleted(lot))
                return Result.Fail("Lot completed");
            if(amount < (lot.CurrentBet?.BetAmount.Amount ?? 0) + lot.MinBetCurrency.Amount)
                return Result.Fail("Wrong amount");
            var user = await context.Users.Include(x=>x.Currencies).FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return Result.Fail("Couldnt find the user");
            if (lot.LotOwner == user)
                return Result.Fail("Lot's owner can't place a bet");
            if (lot.CurrentBet?.BetParticipant == user)
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
                .Include(x=>x.LotOwner)
                    .ThenInclude(x=>x.Currencies)
                .FirstOrDefaultAsync(x => x.Id == lotId);
            if (lot is null)
                return Result.Fail("Couldnt find the lot");
            if (await CheckLotCompleted(lot))
                return Result.Fail("Lot completed");
            if (lot.BuyoutPrice is null)
                return Result.Fail("Lot havent buyout option");
            var user = await context.Users.Include(x => x.Currencies).FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null || user.OriginalId is null)
                return Result.Fail("Couldnt find the user");
            if (lot.LotOwner == user)
                return Result.Fail("Lot's owner can't buyout");

            var withdrawOperation =
                WithdrawMoney(user, lot.BuyoutPrice);
            if (withdrawOperation.Failed)
                return withdrawOperation;
            if ((await apiService.MoveItem(lot.ItemInfo.Id, user.OriginalId)).Failed)
            {
                DepositMoney(user, lot.BuyoutPrice);
                return Result.Fail("Unable to move the item to the new owner");
            }
            DepositMoney(lot.LotOwner, lot.BuyoutPrice);
            ReturnBetMoney(lot);
            lot.ItemInfo.Owner = user;
            await MoveExpiredLotToArchive(lot.Id, LotArchiveEndType.Bought, lot.BuyoutPrice, user);
            await context.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<Result> WithdrawMoney(Guid userId, Money money)
        {
            var user = await context.Users
                .Include(x=>x.Currencies).FirstOrDefaultAsync(x => x.Id == userId);
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
        public async Task<bool> CheckLotCompleted(Guid lotId)
        {
            var lot = context.Lots
            .Include(x => x.ItemInfo)
            .Include(x=>x.LotOwner)
                .ThenInclude(x=>x.Currencies)
            .Include(x=>x.CurrentBet)
                .ThenInclude(x=>x.BetParticipant)
            .FirstOrDefault(x => x.Id == lotId);
            if (lot is null)
                return true;
            return await CheckLotCompleted(lot);
        }
        public async Task<Result<Guid>> CreateNewLot(TimeOnly duration, string itemId, Money minBet, Money? buyoutPrice)
        {
            var item = context.Items
                .Include(x=>x.Owner).FirstOrDefault(x => x.Id == itemId);
            if(item is null)
                return Result<Guid>.Fail("Item not found");
            if (context.Lots.FirstOrDefault(x => x.ItemInfo.Id == itemId) is not null)
                return Result<Guid>.Fail("Lot already exists");
            if((await apiService.HoldItem(itemId)).Failed)
                return Result<Guid>.Fail("Unable to hold the item");
            var lot = context.Lots.Add(new Lot(item, DateTime.Now,
                duration.ToTimeSpan(), minBet, item.Owner, buyoutPrice)).Entity;
            await context.SaveChangesAsync();
            return Result.Ok(lot.Id);
        }
        public async Task<Result> UpdateItemsAndLots(Guid userId)
        {
            var owner = context.Users.FirstOrDefault(x => x.Id == userId);
            if (owner is null || owner.OriginalId is null)
                return Result.Fail();
            var existingItemsResult = await apiService.LoadUserItems(owner.OriginalId);
            if(existingItemsResult.Failed)
                return Result.Fail();
            var existingItems = existingItemsResult.Data!;
            foreach (var item in existingItems)
            {
                if (context.Items.Any(x => x.Id == item.Id.ToString()))
                    continue;
                var poster = item.Poster is null ? null : $"/image/{item.Poster}";
                var newItem = new Item(item.Id, item.Name, item.Description, item.Type, owner, poster);
                context.Items.Add(newItem);
            }
            foreach(var item in context.Items.Where(x=>x.Owner.Id == owner.Id).ToList())
            {
                if (existingItems.Any(x => x.Id == item.Id.ToString()))
                    continue;/*
                var lot = context.Lots.FirstOrDefault(x => x.ItemInfo.Id == item.Id);
                if (lot is not null)
                {
                    ReturnBetMoney(lot);
                    context.Lots.Remove(lot);
                }*/
                context.Items.Remove(item);
            }
            context.SaveChanges();
            return Result.Ok();
        }
        public async Task<Result> CancelLot(Guid lotId)
        {
            var lot = context.Lots
                .Include(x=>x.ItemInfo)
                .Include(x=>x.CurrentBet)
                    .ThenInclude(x=>x.BetParticipant)
                .FirstOrDefault(x => x.Id == lotId);
            if (lot is null)
                return Result.Fail();
            await apiService.UnholdItem(lot.ItemInfo.Id);
            ReturnBetMoney(lot);
            await MoveExpiredLotToArchive(lot.Id, LotArchiveEndType.Canceled);
            await context.SaveChangesAsync();
            return Result.Ok();
        }
        private async Task<bool> CheckLotCompleted(Lot lot)
        {
            if (lot.TimeUntilClosing(DateTime.Now).Ticks < 0)
            {
                if (lot.CurrentBet is not null && lot.CurrentBet.BetParticipant.OriginalId is not null)
                {
                    if((await apiService.MoveItem(lot.ItemInfo.Id, lot.CurrentBet.BetParticipant.OriginalId)).Failed)
                        return true;
                    DepositMoney(lot.LotOwner, lot.CurrentBet.BetAmount);
                    lot.ItemInfo.Owner = lot.CurrentBet.BetParticipant;
                    await MoveExpiredLotToArchive(lot.Id, LotArchiveEndType.Bought, lot.CurrentBet.BetAmount, lot.CurrentBet.BetParticipant);
                }
                else
                    await MoveExpiredLotToArchive(lot.Id, LotArchiveEndType.Expired);
                context.SaveChanges();
                return true;
            }
            return false;
        }
        private async Task MoveExpiredLotToArchive(Guid lotId, LotArchiveEndType endType, Money? boughtFor = null, User? buyer = null)
        {
            var lot = await context.Lots
                .Include(x=>x.ItemInfo)
                .Include(x=>x.LotOwner).FirstOrDefaultAsync(x => x.Id == lotId);
            if (lot is null)
                return;
            await context.ArchivalLots.AddAsync(new ArchivalLot(lot.ItemInfo,
                lot.LotOwner, boughtFor, buyer, endType, DateTime.Now));
            context.Lots.Remove(lot);
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
