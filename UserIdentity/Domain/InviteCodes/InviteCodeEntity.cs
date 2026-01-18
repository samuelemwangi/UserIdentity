using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using PolyzenKit.Domain.Entity;
using PolyzenKit.Domain.RegisteredApps;

namespace UserIdentity.Domain.InviteCodes;

public class InviteCodeEntity : BaseAuditableEntity<long>
{
  [Required]
  public string InviteCode { get; set; } = null!;

  [Required]
  public string UserEmail { get; set; } = null!;

  public int AppId { get; set; }

  [JsonIgnore]
  [ForeignKey(nameof(AppId))]
  public virtual RegisteredAppEntity App { get; set; } = null!;
}
