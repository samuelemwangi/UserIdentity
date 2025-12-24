using PolyzenKit.Domain.Entity;

namespace UserIdentity.Domain.Identity;

public record RefreshTokenEntity : BaseAuditableEntity<Guid>
{
  public string? Token { get; internal set; }

  public DateTime Expires { get; internal set; }

  public string? UserId { get; internal set; }

  public bool? Active => DateTime.UtcNow <= Expires;

  public string? RemoteIpAddress { get; internal set; }
}
