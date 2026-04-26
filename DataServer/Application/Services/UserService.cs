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
    public class UserService : IUserService
    {
        public Result<Guid> Create(User item)
        {
            throw new NotImplementedException();
        }

        public Result DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public Result<User> GetById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
