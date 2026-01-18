using PolyzenKit.Application.Core;

using UserIdentity.Domain.Users;

namespace UserIdentity.Application.Core.Users.ViewModels;

public record ResetPasswordViewModel : BaseViewModel
{
  public ResetPasswordDTO ResetPasswordDetails { get; init; } = null!;
}
