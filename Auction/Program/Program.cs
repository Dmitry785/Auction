using Application;
using Application.Interfaces;
using Application.Logic.User;
using Application.Services;
using Domain.Models;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace Auction
{
    public class Program
    {
        private static bool USE_DEF_DATA = true;
        public static void Main(string[] args)
        {
            if (args.Contains("--use-def-data"))
            {
                USE_DEF_DATA = true;
            }
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddInfastructureLayer(builder.Configuration);

            builder.Services.AddApplicationLayer();

            builder.Services.AddTransient<LoginService>();
            builder.Services.AddTransient<DefaultDataHelper>();
            builder.Services.AddTransient<PaymentService>();
            builder.Services.AddScoped<CmdService>();
            builder.Services.AddSingleton<BanService>();
            builder.Services.AddTransient(p=>new DataServerApiService("address"));

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxConcurrentConnections = 100;
                options.Limits.MaxConcurrentUpgradedConnections = 100;
            });

            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                    context => RateLimitPartition.GetSlidingWindowLimiter(
                        //context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "unknown",
                        _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 200,
                            Window = TimeSpan.FromSeconds(10),
                            SegmentsPerWindow = 2
                        }));
                options.AddFixedWindowLimiter("auth", options =>
                {
                    options.PermitLimit = 3;
                    options.Window = TimeSpan.FromSeconds(15);
                });
            });

            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(p =>
                {
                    p.LoginPath = "/authorization";
                    p.AccessDeniedPath = "/unauthorized";
                });
            builder.Services.AddAuthorization(opts =>
            {
                opts.AddPolicy("LinkedToTheOriginalAccount", p =>
                {
                    p.RequireClaim("linked_account_id");
                });
            });

            builder.Services.AddHostedService<CmdListenerService>();

            var app = builder.Build();

            app.UseMiddleware<DdosProtectionMiddleware>();

            app.UseStaticFiles();

            app.Use(async (c, next) =>
            {
                Console.WriteLine($"{c.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "unknown"} >> {c.Request.Path}");
                /*if (c.Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    Console.WriteLine($"{c.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "unknown"} !! using X-Forwarded-For header");
                    return;
                }*/
                await next.Invoke();
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            app.MapControllers();

            if (USE_DEF_DATA) 
                using (var scope = app.Services.CreateScope())
                    scope.ServiceProvider.GetRequiredService<DefaultDataHelper>().AddDefaultData();

            app.Run();
        }
    }
}
public class DefaultDataHelper
{
    private readonly AppDbContext _context;
    private readonly LoginService _ls;
    public DefaultDataHelper(AppDbContext context, LoginService ls)
    {
        _context = context;
        _ls = ls;
    }
    public void AddDefaultData()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        _context.Users.First(x => x.Id == _ls.TryRegisterAsync("1", "a", "1").Result.Data)
            .Currencies.AddRange(new List<WalletCurrency>(){
                new WalletCurrency(1000, CurrencyType.RUB),
                new WalletCurrency(100000, CurrencyType.BTC)
            });
        _context.Users.First(x => x.Id == _ls.TryRegisterAsync("2", "dani", "1").Result.Data)
            .Currencies.AddRange(new List<WalletCurrency>(){
                new WalletCurrency(1000, CurrencyType.RUB)
            });
        _context.Users.First(x => x.Id == _ls.TryRegisterAsync("3", "sb", "1").Result.Data)
            .Currencies.AddRange(new List<WalletCurrency>(){
                new WalletCurrency(1000, CurrencyType.RUB)
            });
        _context.Users.First(x => x.Id == _ls.TryRegisterAsync("4", "s", "1").Result.Data)
            .Currencies.AddRange(new List<WalletCurrency>(){
                new WalletCurrency(1000, CurrencyType.RUB)
            });
        var user1 = new User("u1", DateTime.Now, "u1 n", "erg", new List<WalletCurrency>() { new WalletCurrency(1111, CurrencyType.RUB)});
        var user2 = new User("u2", DateTime.Now, "u2 n", "egr", new List<WalletCurrency>() { new WalletCurrency(10, CurrencyType.RUB) });
        var item1 = new Item("1", "Item 1", "Item 1 desc", ItemType.Usual, user2, "https://fb.ru/misc/i/gallery/10682/1225582.jpg");
        var item2 = new Item("2", "Item 2", "Item 2 desc", ItemType.GameSkin, user2, "/images/items/csgo.png");
        var item3 = new Item("3", "Item 3", "Item 3 desc", ItemType.Usual, user1);
        var lot1 = new Lot(item1, DateTime.Now, TimeSpan.FromSeconds(50), 
            new Money(10, CurrencyType.RUB), item1.Owner,
            new Money(10000, CurrencyType.BTC));
        var lot2 = new Lot(item2, DateTime.Now, TimeSpan.FromSeconds(150),
            new Money(100, CurrencyType.RUB), item2.Owner);
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
public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        return Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId)
            ? userId : Guid.Empty;
    }
    public static string? GetLinkedAccountId(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("linked_account_id");
    }
}
public class CmdService(PaymentService _paymentService, IMediator _mdtr,
    ILogger<CmdService> _logger)
{

   public async Task ExecuteAsync(string input)
    {
        await HandleCommand(input);
    }
    private async Task HandleCommand(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return;
        var inputStringSplit = input.Split();
        string command = inputStringSplit[0];
        string[] parameters = new string[0];
        if (command.Length > 1)
            parameters = new string[command.Length - 1];
        Array.Copy(inputStringSplit, 1, parameters, 0, inputStringSplit.Length - 1);
        try
        {
            switch (command)
            {
                case "deposit":
                    var username = parameters[0];
                    var moneyAmount = decimal.Parse(parameters[1]);
                    var moneyType = GetCurrencyType(parameters[2]);
                    var result = await _paymentService.DepositMoney(await GetUserIdByUsername(username),
                        new Money(moneyAmount, moneyType));
                    if (result.Failed)
                    {
                        _logger.LogWarning($"Command \"{command}\" >> {result.ErrorMessage}");
                        return;
                    }
                    break;
                case "withdraw":
                    username = parameters[0];
                    moneyAmount = decimal.Parse(parameters[1]);
                    moneyType = GetCurrencyType(parameters[2]);
                    result = await _paymentService.WithdrawMoney(await GetUserIdByUsername(username),
                        new Money(moneyAmount, moneyType));
                    if (result.Failed)
                    {
                        _logger.LogWarning($"Command \"{command}\" >> {result.ErrorMessage}");
                        return;
                    }
                    break;
                default:
                    _logger.LogWarning($"Command \"{command}\" not found");
                    return;
            }
            _logger.LogWarning($"Successful execution command \"{command}\"");
        }
        catch
        {
            _logger.LogWarning($"Failed to execute command \"{input}\"");
        }
    }
    private async Task<Guid> GetUserIdByUsername(string username)
    {
        return (await _mdtr.Send(new GetAllUsersQuery(x => x.Username == username))).First().Id;
    }
    private CurrencyType GetCurrencyType(string type)
    {
        switch (type)
        {
            case "rub":
                return CurrencyType.RUB;
            case "usd":
                return CurrencyType.USD;
            case "btc":
                return CurrencyType.BTC;
            case "eth":
                return CurrencyType.ETH;
        }
        throw new Exception("parse exception");
    }
}
public class CmdListenerService(IServiceProvider _serviceProvider,
    ILogger<CmdListenerService> _logger) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cmd Service has started");
        await Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var input = await Task.Run(() => Console.ReadLine(), cancellationToken);
                if (string.IsNullOrEmpty(input)) continue;
                var scope = _serviceProvider.CreateScope();
                await scope.ServiceProvider.GetRequiredService<CmdService>().ExecuteAsync(input);
            }
        }, cancellationToken);
    }
}