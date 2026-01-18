using PolyzenKit.Application.Core;

using UserIdentity.Domain.Users;

namespace UserIdentity.Application.Core.Users.ViewModels;

public record UpdatePasswordViewModel : BaseViewModel
{
  public UpdatePasswordDTO UpdatePasswordResult { get; init; } = null!;
}

