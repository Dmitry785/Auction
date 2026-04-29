using Application.Common;
using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LoginService(IAppDbContext context)
    {
        public async Task<Result<Guid>> TryLoginAsync(string username, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Username == username &&
            x.PasswordHash == GetHashPassword(password));
            if (user is null)
                return Result<Guid>.Fail();
            return Result<Guid>.Ok(user.Id);
        }
        public async Task<Result<Guid>> TryRegisterAsync(string username, string password, string name)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Username ==username);
            if (user is not null)
                return Result<Guid>.Fail();
            user = (await context.Users.AddAsync(new User(username, DateTime.Now,
                name, GetHashPassword(password), new List<WalletCurrency>()))).Entity;
            await context.SaveChangesAsync();
            return Result<Guid>.Ok(user.Id);
        }
        public async Task LinkUser(Guid userId, string originalId)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return;
            user.OriginalId = originalId;
            await context.SaveChangesAsync();
        }
        private string GetHashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = SHA256.HashData(bytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}
