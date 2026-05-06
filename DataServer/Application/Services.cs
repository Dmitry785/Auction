using Application.Common;
using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;
public class CommonService(IAppDbContext context)
{
    public List<Item> GetAllItems()
    {
        return context.Items.AsNoTracking().ToList();
    }
    public Result<List<Item>> GetAllUserItems(Guid userId)
    {
        var user = context.Users.FirstOrDefault(x => x.Id == userId);
        if (user is null)
            return Result<List<Item>>.Fail();
        return Result.Ok(context.Items.AsNoTracking().Where(x => x.Owner == user).ToList());
    }
    public Result HoldItem(Guid itemId)
    {
        var item = context.Items.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
            return Result.Fail();
        item.IsHolding = true;
        context.SaveChanges();
        return Result.Ok();
    }
    public Result UnholdItem(Guid itemId)
    {
        var item = context.Items.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
            return Result.Fail();
        item.IsHolding = false;
        context.SaveChanges();
        return Result.Ok();
    }
    public Result MoveItem(Guid itemId, Guid ownerId)
    {
        var item = context.Items.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
            return Result.Fail();
        var user = context.Users.FirstOrDefault(x => x.Id == ownerId);
        if (user is null)
            return Result.Fail();
        if(item.IsHolding)
            return Result.Fail();
        item.Owner = user;
        context.SaveChanges();
        return Result.Ok();
    }
    public Result<Guid> GetUserId(string username, string password)
    {
        var user = context.Users.FirstOrDefault(x => x.Username == username &&
            x.Password == password);
        if (user is null)
            return Result<Guid>.Fail();
        return Result.Ok(user.Id);
    }
}
