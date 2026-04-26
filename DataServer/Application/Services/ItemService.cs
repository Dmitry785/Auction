using Application.Common;
using Application.Services.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ItemService : IItemService
    {
        public Result<Guid> Create(Item item)
        {
            throw new NotImplementedException();
        }

        public Result DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<Item> GetAll()
        {
            throw new NotImplementedException();
        }

        public Result<Item> GetById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
