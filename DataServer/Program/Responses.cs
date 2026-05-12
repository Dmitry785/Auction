using Domain.Models;

namespace Program
{
    public sealed record UserData(string id, string name, string username);
    public sealed record ItemData(string id, string name, string description, ItemType type, string ownerId, string? poster);
}
