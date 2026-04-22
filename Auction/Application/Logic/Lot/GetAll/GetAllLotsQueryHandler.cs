using Application.Common;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic.Lot.GetAll
{
    public sealed record GetAllLotsQueryHandler(IAppDbContext context) : IRequestHandler<GetAllLotsQuery, Result<List<Domain.Models.Lot>>>
    {
        public async Task<Result<List<Domain.Models.Lot>>> Handle(GetAllLotsQuery request, CancellationToken cancellationToken)
        {
            var lots = await context.Lots.ToListAsync();
            return Result.Ok(lots);
        }
    }
}
