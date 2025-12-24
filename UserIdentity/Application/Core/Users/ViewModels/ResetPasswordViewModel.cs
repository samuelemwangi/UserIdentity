using PolyzenKit.Application.Core;

namespace UserIdentity.Application.Core.Users.ViewModels;

public record ResetPasswordDTO
{
  public string EmailMessage { get; init; } = null!;
}
public record ResetPasswordViewModel : BaseViewModel
{
  public ResetPasswordDTO ResetPasswordDetails { get; init; } = null!;
}

