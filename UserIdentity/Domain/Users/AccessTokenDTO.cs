namespace UserIdentity.Domain.Users;

public record AccessTokenDTO
{
  public string? Token { get; init; }

  public int ExpiresIn { get; init; }
}
