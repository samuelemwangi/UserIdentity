using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

using PolyzenKit.Domain.DTO;
using PolyzenKit.Domain.Entity;

namespace UserIdentity.Domain.Identity;

public record RegisteredAppEntity : BaseAuditableEntity<int>
{
	[Required]
	[StringLength(100)]
	public string AppName { get; set; } = null!;

	[Required]
	[StringLength(100)]
	public string AppSecretKey { get; set; } = null!;

	[StringLength(600)]
	public string? CallbackUrl { get; set; }

	public bool ForwardServiceToken { get; set; }

	public Dictionary<string, string>? CallbackHeaders { get; set; }

	[JsonIgnore]
	public virtual ICollection<UserRegisteredAppEntity> UserRegisteredApps { get; set; } = [];
}

public record RegisteredAppDTO : RegisteredAppEntity, IBaseAuditableEntityDTO<int>
{
	public static Expression<Func<RegisteredAppEntity, RegisteredAppDTO>> Basic =>
		entity => new RegisteredAppDTO
		{
			Id = entity.Id,
			AppName = entity.AppName,
			AppSecretKey = entity.AppSecretKey,
			CallbackUrl = entity.CallbackUrl,
			ForwardServiceToken = entity.ForwardServiceToken,
			CallbackHeaders = entity.CallbackHeaders,
			CreatedBy = entity.CreatedBy,
			CreatedAt = entity.CreatedAt,
			UpdatedBy = entity.UpdatedBy,
			UpdatedAt = entity.UpdatedAt
		};
}

