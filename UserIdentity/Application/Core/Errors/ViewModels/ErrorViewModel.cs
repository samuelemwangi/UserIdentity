namespace UserIdentity.Application.Core.Errors.ViewModels
{
  public record ErrorViewModel : BaseViewModel
  {
    public ErrorDTO? Error { get; internal set; }
  }
}
