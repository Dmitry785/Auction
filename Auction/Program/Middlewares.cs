using System.Collections.Concurrent;

public class DdosProtectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly BanService _banService;
    private static readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _trackers = new();
    private readonly int _secondsBanDuration = 20;
    private readonly int _maxRequestsPerTime = 50;
    private readonly int _requestsCounterSecondsTimeLimit = 60;
    public DdosProtectionMiddleware(RequestDelegate next, BanService banService)
    {
        _next = next;
        _banService = banService;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "unknown";

        var isBannedResult = _banService.IsBanned(ip);
        if (isBannedResult.Success)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync($"You have banned (wait until {isBannedResult.Data!.ToString()})");
            return;
        }

        var now = DateTime.UtcNow;
        var tracker = _trackers.AddOrUpdate(ip, (1, now), (key, old) =>
            (now - old.WindowStart).TotalSeconds < _requestsCounterSecondsTimeLimit ? (old.Count + 1, old.WindowStart) : (1, now));

        if (tracker.Count > _maxRequestsPerTime)
        {
            _banService.Ban(ip, TimeSpan.FromSeconds(_secondsBanDuration));
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync($"You have banned (wait until {isBannedResult.Data!.ToString()})");
            return;
        }

        await _next(context);
    }
}
