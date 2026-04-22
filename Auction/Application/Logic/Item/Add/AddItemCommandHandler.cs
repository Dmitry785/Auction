using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Logic.Item.Add
{
    public sealed record AddItemCommandHandler(IAppDbContext context) : IRequestHandler<AddItemCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(AddItemCommand request, CancellationToken cancellationToken)
        {
            var entity = (await context.Items.AddAsync(request.item)).Entity;
            return Result.Ok(entity.Id);
        }
    }
}
