using Application.Common;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic.User;

public sealed record GetAllUsersQuery(Func<Domain.Models.User, bool>? predicate = null) : IRequest<List<Domain.Models.User>>;
