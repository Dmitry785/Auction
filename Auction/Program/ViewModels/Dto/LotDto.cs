namespace Program.ViewModels.Dto;

public sealed record LotDto(DateTime ExpiresAt, Guid Id, Guid OwnerId, string ItemName, string OwnerUsername, string ItemDescription, string? Poster);
public sealed record ArchivalLotDto(Guid Id, Guid OwnerId, string ItemName, string OwnerUsername, string ItemDescription, string? Poster);
/*public Item ItemInfo { get; set; } = null!;
        public User LotOwner { get; set; } = null!;
        public LotArchiveEndType EndType { get; set; }
        public Money? BoughtFor { get; set; }
        public DateTime CompletionTime { get; set; }*/