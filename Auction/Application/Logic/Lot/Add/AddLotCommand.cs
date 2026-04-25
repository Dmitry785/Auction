using Application.Common;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic.Lot;

public sealed record AddLotCommand(Domain.Models.Lot Lot) : IRequest<Result<Guid>>;
