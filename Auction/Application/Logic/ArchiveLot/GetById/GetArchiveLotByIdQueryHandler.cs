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

namespace Application.Logic.ArchiveLot;

public sealed record GetArchiveLotByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetArchiveLotByIdQuery, Result<ArchivalLot>>
{
    public async Task<Result<ArchivalLot>> Handle(GetArchiveLotByIdQuery request, CancellationToken cancellationToken)
    {
        var lot = await context.ArchivalLots
            .Include(x => x.ItemInfo)
                .ThenInclude(x => x.Owner)
            .Include(x => x.LotOwner)
            .Include(x=>x.Buyer)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id);
        if (lot is null)
            return Result<ArchivalLot>.Fail();
        return Result.Ok(lot);
    }
}
