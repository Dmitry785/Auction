using Program.ViewModels.Dto;

namespace Program.ViewModels;

public sealed record CurrentUserItemsViewModel(List<CurrentUserItemDto> Items);
