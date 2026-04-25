using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Logic.Item;

public sealed record DeleteItemByIdCommandHandler(IAppDbContext context) : IRequestHandler<DeleteItemByIdCommand, Result>
{
    public async Task<Result> Handle(DeleteItemByIdCommand request, CancellationToken cancellationToken)
    {
        var item = context.Items.FirstOrDefault(x => x.Id == request.Id);
        if (item is null)
            return Result.Fail();
        context.Items.Remove(item);
        await context.SaveChangesAsync();
        return Result.Ok();
    }
}
