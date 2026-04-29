using Domain.Models;

namespace Program.ViewModels
{
    public sealed record LotViewModel(string ItemName, string Description, 
        string? Poster, ItemType ItemType, string OwnerName, Money? BuyoutPrice,
        Money MinBetCurrency, Money? LastBetAmount, string? LastBetUserName);
}
