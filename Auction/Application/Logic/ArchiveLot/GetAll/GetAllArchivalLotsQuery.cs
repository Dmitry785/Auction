using Application.Common;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic.ArchiveLot;

public sealed record GetAllArchivalLotsQuery(Func<ArchivalLot, bool>? predicate = null) : IRequest<List<ArchivalLot>>;
