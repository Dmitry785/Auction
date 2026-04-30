namespace Program.ViewModels.Dto;

public sealed record LotDto(DateTime ExpiresAt, Guid Id, string ItemName, string OwnerUsername, string ItemDescription, string? Poster);