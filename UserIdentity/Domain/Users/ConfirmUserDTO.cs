namespace UserIdentity.Domain.Users;

public record ConfirmUserDTO
{
  public bool UserConfirmed { get; init; }
}
