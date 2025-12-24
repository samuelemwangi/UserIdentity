using PolyzenKit.Application.Core;

namespace UserIdentity.Application.Core.Users.ViewModels;

public record ConfirmUserDTO
{
  public bool UserConfirmed { get; init; }
}
public record ConfirmUserViewModel : ItemDetailBaseViewModel
{
  public required ConfirmUserDTO ConfirmUserResult { get; init; }
}
