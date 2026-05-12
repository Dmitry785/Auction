using Application.Interfaces;
using Application.Services;
using Domain.Models;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DataServer;

public class CmdService(AppDbContext context,
    ILogger<CmdService> _logger)
{
    private readonly HttpClient _client = new HttpClient()
    {
        DefaultRequestHeaders = { { "User-Agent", "MyAuctionDataServer/1.0 (contact: danchenko.d2020@gmail.com) .NET-HttpClient" } }
    };

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
            parameters = new string[inputStringSplit.Length - 1];
        Array.Copy(inputStringSplit, 1, parameters, 0, inputStringSplit.Length - 1);
        try
        {
            switch (command)
            {
                case "show":
                    switch (parameters[0])
                    {
                        case "items":
                            foreach (var item in context.Items.Include(x => x.Owner))
                            {
                                Console.WriteLine($"Id\t\t{item.Id}\nName\t\t{item.Name}\nOwner\t\t{item.Owner.Id}" +
                                    $"\nPoster\t\t{item.Poster}\nDescription\t{item.Description}\n");
                            }
                            break;
                        case "users":
                            foreach (var user in context.Users)
                            {
                                Console.WriteLine($"Id\t\t{user.Id}\nName\t\t{user.Name}\nUsername\t{user.Username}" +
                                    $"\nPassword\t{user.Password}\n");
                            }
                            break;
                        default:
                            Console.WriteLine("show {items, users}");
                            break;
                    }
                    break;
                case "add":
                    switch (parameters[0])
                    {
                        case "item":
                            var item = new Item(parameters[1], parameters[2], 
                                (ItemType)int.Parse(parameters[3]), Guid.Parse(parameters[4]));
                            if (parameters.Length > 5)
                            {
                                var imagePathGuid = Guid.NewGuid().ToString();
                                item.Poster = imagePathGuid + Path.GetExtension(parameters[5]);
                                if (Uri.TryCreate(parameters[5], UriKind.Absolute, out Uri? uriResult)
                                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                                {
                                    var bytes = await _client.GetByteArrayAsync(parameters[5]);

                                    await File.WriteAllBytesAsync(Path.Combine("wwwroot/images",
                                    imagePathGuid + Path.GetExtension(parameters[5])), bytes);
                                }
                                else if (File.Exists(parameters[5]))
                                {
                                    File.Copy(parameters[5], Path.Combine("wwwroot/images",
                                    imagePathGuid + Path.GetExtension(parameters[5])));
                                }
                                
                            }
                            context.Items.Add(item);
                            context.SaveChanges();
                            break;
                        case "user":
                            var user = new User(parameters[1], parameters[2],
                                parameters[3], new List<Item>());
                            context.Users.Add(user);
                            context.SaveChanges();
                            break;
                        default:
                            Console.WriteLine("add item {name} {desc} {type} {ownerId} {poster}");
                            Console.WriteLine("add user {name} {username} {password}");
                            break;
                    }
                    break;
                case "reset":
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
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