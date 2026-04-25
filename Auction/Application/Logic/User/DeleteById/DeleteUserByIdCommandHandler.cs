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

public sealed record DeleteUserByIdCommandHandler(IAppDbContext context) : IRequestHandler<DeleteUserByIdCommand, Result>
{
    public async Task<Result> Handle(DeleteUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = context.Users.FirstOrDefault(x => x.Id == request.Id);
        if (user is null)
            return Result.Fail();
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return Result.Ok();
    }
}
