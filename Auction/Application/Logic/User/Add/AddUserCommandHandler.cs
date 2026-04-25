using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Logic.User;

public sealed record AddUserCommandHandler(IAppDbContext context) : IRequestHandler<AddUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var entity = (await context.Users.AddAsync(request.User)).Entity;
        await context.SaveChangesAsync();
        return Result.Ok(entity.Id);
    }
}
