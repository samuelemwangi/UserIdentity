using System.Text.Json.Serialization;

using PolyzenKit.Domain.DTO;
using PolyzenKit.Domain.Entity;

namespace UserIdentity.Domain.Users;

public class UserDTO : BaseAuditableEntity<string>, IBaseAuditableEntityDTO<string>
{
  public string? FirstName { get; init; }

  public string? LastName { get; init; }

  public string? UserName { get; init; }

  public string? Email { get; init; }

  public string? PhoneNumber { get; init; }

  [JsonIgnore]
  public HashSet<string> Roles { get; init; } = [];

  [JsonIgnore]
  public HashSet<string> RoleClaims { get; init; } = [];
}
