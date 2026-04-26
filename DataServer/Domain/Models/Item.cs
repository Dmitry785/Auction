using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models;

public class Item : BaseModel<Guid>
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? Poster { get; set; }
    public ItemType Type { get; set; }
    public User Owner { get; set; } = null!;
    public Item(string name, string description, ItemType type, User owner, string? poster = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Poster = poster;
        Type = type;
        Owner = owner;
    }
    public Item()
    {
    }
}
public enum ItemType
{
    GameSkin,
    Nft,
    Service,
    Product,
    Usual,
    Drug,
    Other
}

