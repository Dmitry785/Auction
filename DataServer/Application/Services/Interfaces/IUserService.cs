using Application.Common;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        Result<User> GetById(Guid id);
        List<User> GetAll();
        Result DeleteById(Guid id);
        Result<Guid> Create(User item);
    }
}
