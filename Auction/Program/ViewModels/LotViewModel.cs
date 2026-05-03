using Domain.Models;

namespace Program.ViewModels;

public sealed record LotViewModel(Guid Id, string ItemId, Guid OwnerId, string ItemName, string Description, 
    string? Poster, ItemType ItemType, string OwnerName, Money? BuyoutPrice,
    Money MinBetCurrency, Money? LastBetAmount, string? LastBetUsername,
    DateTime ExpiresAt, bool IsUserAuthorized, bool HasUserLinkedAccount, bool CanUserBet, bool CanUserBuyout,
    decimal? MinBet, decimal? MaxBet, bool IsExpired);
