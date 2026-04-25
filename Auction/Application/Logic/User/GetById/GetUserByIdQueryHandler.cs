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

namespace Application.Logic.User;

public sealed record GetUserByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetUserByIdQuery, Result<Domain.Models.User>>
{
    public async Task<Result<Domain.Models.User>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (user is null)
            return Result<Domain.Models.User>.Fail();
        return Result.Ok(user);
    }
}
