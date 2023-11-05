namespace UserIdentity.Application.Core.Users.ViewModels
{
  public record ResetPasswordDTO
  {
    public String EmailMessage { get; init; }
  }
  public record ResetPasswordViewModel : BaseViewModel
  {
    public ResetPasswordDTO ResetPasswordDetails { get; init; }
  }
}

