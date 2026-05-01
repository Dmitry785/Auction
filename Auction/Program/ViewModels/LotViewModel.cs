using Domain.Models;

namespace Program.ViewModels;

public sealed record LotViewModel(Guid Id, string ItemName, string Description, 
    string? Poster, ItemType ItemType, string OwnerName, Money? BuyoutPrice,
    Money MinBetCurrency, Money? LastBetAmount, string? LastBetUserName,
    DateTime ExpiresAt, bool IsUserAuthorized, bool HasUserLinkedAccount, bool CanUserBet, bool CanUserBuyout,
    decimal? MinBet, decimal? MaxBet);
