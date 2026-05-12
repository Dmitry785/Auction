using Application.Interfaces;
using Application.Services;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Auction;

public class CmdService(LotsManagementAndPaymentService _paymentService, IAppDbContext context,
    ILogger<CmdService> _logger, DefaultDataHelper _defaultDataHelper)
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
                case "add":
                    _defaultDataHelper.AddDefaultData();
                    break;
                case "reset":
                    _defaultDataHelper.ResetDatabase();
                    break;
                case "clear-archive":
                    context.ArchivalLots.ExecuteDelete();
                    context.SaveChanges();
                    break;
                default:
                    _logger.LogWarning($"Command \"{command}\" not found");
                    return;
            }
            _logger.LogInformation($"Successful execution command \"{command}\"");
        }
        catch
        {
            _logger.LogWarning($"Failed to execute command \"{input}\"");
        }
    }
    private async Task<Guid> GetUserIdByUsername(string username)
    {
        return (await context.Users.FirstAsync(x => x.Username == username)).Id;
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