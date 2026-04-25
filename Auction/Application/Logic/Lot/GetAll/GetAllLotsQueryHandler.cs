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

namespace Application.Logic.Lot;

public sealed record GetAllLotsQueryHandler(IAppDbContext context) : IRequestHandler<GetAllLotsQuery, List<Domain.Models.Lot>>
{
    public async Task<List<Domain.Models.Lot>> Handle(GetAllLotsQuery request, CancellationToken cancellationToken)
    {
        var lots = await context.Lots.AsNoTracking().ToListAsync();
        if (request.predicate is not null)
            lots = lots.Where(request.predicate!).ToList();
        return lots;
    }
}
