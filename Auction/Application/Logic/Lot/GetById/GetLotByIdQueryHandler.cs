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

public sealed record GetLotByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetLotByIdQuery, Result<Domain.Models.Lot>>
{
    public async Task<Result<Domain.Models.Lot>> Handle(GetLotByIdQuery request, CancellationToken cancellationToken)
    {
        var lot = await context.Lots.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (lot is null)
            return Result<Domain.Models.Lot>.Fail();
        return Result.Ok(lot);
    }
}
