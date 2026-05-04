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

public sealed record GetAllArchivalLotsQueryHandler(IAppDbContext context) : IRequestHandler<GetAllArchivalLotsQuery, List<ArchivalLot>>
{
    public async Task<List<ArchivalLot>> Handle(GetAllArchivalLotsQuery request, CancellationToken cancellationToken)
    {
        var lots = await context.ArchivalLots
            .Include(x=>x.ItemInfo)
                .ThenInclude(x=>x.Owner)
            .Include(x => x.LotOwner)
            .Include(x => x.Buyer)
            .AsNoTracking().ToListAsync();
        if (request.predicate is not null)
            lots = lots.Where(request.predicate!).ToList();
        return lots;
    }
}
