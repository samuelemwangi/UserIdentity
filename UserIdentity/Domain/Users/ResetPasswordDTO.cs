namespace UserIdentity.Domain.Users;

public record ResetPasswordDTO
{
  public string EmailMessage { get; init; } = null!;
}
