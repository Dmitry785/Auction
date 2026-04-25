using Application.Common;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic.Item;

public sealed record AddItemCommand(Domain.Models.Item Item) : IRequest<Result<string>>;
