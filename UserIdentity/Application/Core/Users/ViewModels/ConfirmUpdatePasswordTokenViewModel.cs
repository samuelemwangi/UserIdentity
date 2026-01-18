using PolyzenKit.Application.Core;

using UserIdentity.Domain.Users;

namespace UserIdentity.Application.Core.Users.ViewModels;

public record ConfirmUpdatePasswordTokenViewModel : BaseViewModel
{
  public ConfirmUpdatePasswordDTO TokenPasswordResult { get; init; } = null!;

}
