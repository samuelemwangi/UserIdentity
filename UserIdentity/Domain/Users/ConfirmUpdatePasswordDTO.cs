namespace UserIdentity.Domain.Users;

public record ConfirmUpdatePasswordDTO
{
  public bool UpdatePasswordTokenConfirmed { get; init; }
}
