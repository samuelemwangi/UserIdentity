namespace UserIdentity.Application.Core.Errors.ViewModels
{
  public record ErrorDTO
  {
    public String? Message { get; internal set; }
    public DateTime? Timestamp { get; internal set; }
  }
}
