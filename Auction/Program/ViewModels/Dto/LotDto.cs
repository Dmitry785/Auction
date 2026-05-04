using Domain.Models;

namespace Program.ViewModels.Dto;

public sealed record LotDto(DateTime ExpiresAt, Guid Id, Guid OwnerId, string ItemName, string OwnerUsername, string ItemDescription, string? Poster);
public sealed record ArchivalLotDto(Guid Id, string ItemId, Guid OwnerId, string EndType, Money? BoughtFor, User? Buyer, DateTime CompletionTime, string ItemName, string OwnerUsername, string ItemDescription, string ItemType, string? Poster);
public sealed record PlainArchivalLotDto(Guid Id, Guid OwnerId, DateTime CompletionTime, string ItemName, string OwnerUsername, string ItemDescription, string? Poster);