using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using PolyzenKit.Domain.Entity;
using PolyzenKit.Domain.RegisteredApps;

namespace UserIdentity.Domain.Identity;

public record UserRegisteredAppEntity : BaseAuditableEntity<string>
{
    public int AppId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(AppId))]
    public virtual RegisteredAppEntity App { get; set; } = null!;

    public string UserId { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(UserId))]
    public virtual UserEntity User { get; set; } = null!;
}
