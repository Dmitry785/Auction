using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Logic.Item;

public sealed record GetItemByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetItemByIdQuery, Result<Domain.Models.Item>>
{
    public async Task<Result<Domain.Models.Item>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await context.Items.Include(x=>x.Owner).FirstOrDefaultAsync(x => x.Id == request.Id);
        if (item is null)
            return Result<Domain.Models.Item>.Fail();
        return Result.Ok(item);
    }
}
