using Application.Common;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic.User;

public sealed record AddUserCommand(Domain.Models.User User) : IRequest<Result<Guid>>;
