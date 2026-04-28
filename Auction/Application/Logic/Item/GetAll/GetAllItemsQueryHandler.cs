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

public sealed record GetAllItemsQueryHandler(IAppDbContext context) : IRequestHandler<GetAllItemsQuery, List<Domain.Models.Item>>
{
    public async Task<List<Domain.Models.Item>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await context.Items.Include(x => x.Owner).AsNoTracking().ToListAsync();
        if (request.predicate is not null)
            items = items.Where(request.predicate!).ToList();
        return items;
    }
}
