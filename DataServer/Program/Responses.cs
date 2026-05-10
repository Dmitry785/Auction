using Domain.Models;

namespace Program
{
    public sealed record UserData(Guid id, string name, string username);
    public sealed record ItemData(Guid id, string name, string description, bool isHolding, ItemType type, Guid ownerId, string? poster);
}
