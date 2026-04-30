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
