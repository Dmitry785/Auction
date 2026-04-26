using Application.Common;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IItemService
    {
        Result<Item> GetById(Guid id);
        List<Item> GetAll();
        Result DeleteById(Guid id);
        Result<Guid> Create(Item item);
    }
}
