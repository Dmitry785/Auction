using Program.ViewModels.Dto;

namespace Program.ViewModels;

public sealed record CreateLotViewModel(string ItemId, CreateLotRequest LotData);
