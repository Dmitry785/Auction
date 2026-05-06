using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Item> Items { get; set; }
    int SaveChanges();
}