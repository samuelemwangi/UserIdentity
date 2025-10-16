using PolyzenKit.Domain.Entity;
using System.Text.Json.Serialization;

namespace UserIdentity.Domain.Identity;

public record UserEntity : BaseAuditableEntity<string>
{
	public string? FirstName { get; internal set; }

	public string? LastName { get; internal set; }

	public string? EmailConfirmationToken { get; internal set; }

	public string? ForgotPasswordToken { get; internal set; }

	//[JsonIgnore]
	//public virtual ICollection<UserRegisteredAppEntity> UserRegisteredApps { get; set; } = [];
}
