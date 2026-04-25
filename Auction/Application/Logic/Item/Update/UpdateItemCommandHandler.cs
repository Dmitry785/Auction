using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Logic.Item;

public sealed record UpdateItemCommandHandler(IAppDbContext context) : IRequestHandler<UpdateItemCommand, Result>
{
    public async Task<Result> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        return Result.Fail("В разработке");
    }
}
