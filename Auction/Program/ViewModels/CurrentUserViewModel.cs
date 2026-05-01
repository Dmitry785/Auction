using Domain.Models;

namespace Program.ViewModels;

public sealed record CurrentUserViewModel(string Username, string Name, DateTime RegisterDate, List<WalletCurrency> Currencies);
