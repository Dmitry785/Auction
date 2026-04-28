using Application;
using Application.Interfaces;
using Application.Logic.User;
using Application.Services;
using Domain.Models;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace Auction
{
    public class Program
    {
        private static readonly bool USE_DEF_DATA = false;
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddInfastructureLayer(builder.Configuration);

            builder.Services.AddApplicationLayer();

            builder.Services.AddTransient<LoginService>();

            builder.Services.AddTransient<DefaultDataHelper>();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(p =>
                {
                    p.LoginPath = "/authorization";
                });

            var app = builder.Build();

            app.UseStaticFiles();

            app.MapControllers();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            if(USE_DEF_DATA) 
                using (var scope = app.Services.CreateScope())
                    scope.ServiceProvider.GetRequiredService<DefaultDataHelper>().AddDefaultData();

            app.Run();
        }
    }
}
public class DefaultDataHelper
{
    private readonly AppDbContext _context;
    public DefaultDataHelper(AppDbContext context)
    {
        _context = context;
    }
    public void AddDefaultData()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        var user1 = new User("u1", "u1 n", "erg", new List<WalletCurrency>() { new WalletCurrency(1111, CurrencyType.RUB)});
        var user2 = new User("u2", "u2 n", "egr", new List<WalletCurrency>() { new WalletCurrency(10, CurrencyType.RUB) });
        var item1 = new Item("1", "Item 1", "Item 1 desc", ItemType.Usual, user2);
        var item2 = new Item("2", "Item 2", "Item 2 desc", ItemType.GameSkin, user2);
        var item3 = new Item("3", "Item 3", "Item 3 desc", ItemType.Usual, user1);
        var lot1 = new Lot(item1, DateTime.Now, TimeSpan.FromHours(12), new Money(10, CurrencyType.RUB));
        var lot2 = new Lot(item2, DateTime.Now, TimeSpan.FromSeconds(30), new Money(100, CurrencyType.RUB));
        _context.Users.Add(user1);
        _context.Users.Add(user2);
        _context.Items.Add(item1);
        _context.Items.Add(item2);
        _context.Items.Add(item3);
        _context.Lots.Add(lot1);
        _context.Lots.Add(lot2);
        _context.SaveChanges();
    }
}
