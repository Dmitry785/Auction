using Application.Common;
using MediatR;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic.Lot.GetAll
{
    public sealed record GetAllLotsQuery() : IRequest<Result<List<Domain.Models.Lot>>>;
}
