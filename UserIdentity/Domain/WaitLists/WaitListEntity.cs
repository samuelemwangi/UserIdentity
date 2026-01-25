using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using PolyzenKit.Domain.Entity;
using PolyzenKit.Domain.RegisteredApps;

namespace UserIdentity.Domain.WaitLists;

public class WaitListEntity : BaseAuditableEntity<long>
{
  [Required]
  public string UserEmail { get; set; } = null!;

  public int AppId { get; set; }

  [JsonIgnore]
  [ValidateNever]
  [ForeignKey(nameof(AppId))]
  public virtual RegisteredAppEntity App { get; set; } = null!;
}
