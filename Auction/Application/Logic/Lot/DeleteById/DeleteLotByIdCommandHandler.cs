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

public sealed record DeleteLotByIdCommandHandler(IAppDbContext context) : IRequestHandler<DeleteLotByIdCommand, Result>
{
    public async Task<Result> Handle(DeleteLotByIdCommand request, CancellationToken cancellationToken)
    {
        var lot = context.Lots.FirstOrDefault(x => x.Id == request.Id);
        if (lot is null)
            return Result.Fail();
        context.Lots.Remove(lot);
        await context.SaveChangesAsync();
        return Result.Ok();
    }
}
