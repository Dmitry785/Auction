using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace Infrastructure
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Lot> Lots { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
