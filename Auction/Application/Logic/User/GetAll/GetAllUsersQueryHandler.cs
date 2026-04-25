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

public sealed record GetAllUsersQueryHandler(IAppDbContext context) : IRequestHandler<GetAllUsersQuery, List<Domain.Models.User>>
{
    public async Task<List<Domain.Models.User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await context.Users.AsNoTracking().ToListAsync();
        if (request.predicate is not null)
            users = users.Where(request.predicate!).ToList();
        return users;
    }
}
