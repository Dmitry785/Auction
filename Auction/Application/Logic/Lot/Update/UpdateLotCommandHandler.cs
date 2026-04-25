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

public sealed record UpdateLotCommandHandler(IAppDbContext context) : IRequestHandler<UpdateLotCommand, Result>
{
    public async Task<Result> Handle(UpdateLotCommand request, CancellationToken cancellationToken)
    {
        return Result.Fail("В разработке");
    }
}
