using Domain.Models;

namespace Program.Requests
{
    public sealed record LoginRequest(string Username, string Password);
    public sealed record RegisterRequest(string Username, string Password, string Name);
    public sealed record AddItemRequest(Guid OwnerId, string Name, string Description, string? Poster, ItemType ItemType);
    public sealed record MoveItemRequest(Guid ItemId, Guid NewOwnerId);
}
