namespace Program.ViewModels.Dto;

public sealed record ItemDto(string ItemName, string ItemDescription, 
    string? LotUrl, string? Poster, string OwnerUsername, string ItemType);
public sealed record CurrentUserItemDto(string ItemName, string? Poster, string ItemType, string ItemId);
