namespace UserIdentity.Application.Core.Users.ViewModels
{
  public record ConfirmUpdatePasswordDTO
  {
    public Boolean UpdatePasswordTokenConfirmed { get; init; }
  }

  public record ConfirmUpdatePasswordTokenViewModel : BaseViewModel
  {
    public ConfirmUpdatePasswordDTO TokenPasswordResult { get; init; }

  }
}

