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
        public DbSet<User> Users { get; set; }
        public DbSet<ArchivalLot> ArchivalLots { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(x => x.OriginalId).IsUnique();
            modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();
            modelBuilder.Entity<Lot>().OwnsOne(l => l.CurrentBet, bet =>
            {
                bet.HasOne(b => b.BetParticipant)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict); 
            });
            modelBuilder.Entity<Lot>()
                .HasOne(l => l.ItemInfo)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lot>()
                .HasOne(l => l.LotOwner)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
