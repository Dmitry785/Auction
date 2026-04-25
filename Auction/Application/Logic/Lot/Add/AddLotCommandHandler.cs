using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Logic.Lot;

public sealed record AddLotCommandHandler(IAppDbContext context) : IRequestHandler<AddLotCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddLotCommand request, CancellationToken cancellationToken)
    {
        var entity = (await context.Lots.AddAsync(request.Lot)).Entity;
        await context.SaveChangesAsync();
        return Result.Ok(entity.Id);
    }
}
