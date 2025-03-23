using PolyzenKit.Application.Core;
using PolyzenKit.Domain.DTO;
using PolyzenKit.Domain.Entity;

namespace UserIdentity.Application.Core.Users.ViewModels;

public record UserDTO : BaseAuditableEntity<string>, IBaseAuditableEntityDTO<string>
{
	public string? FullName { get; init; }

	public string? UserName { get; init; }

	public string? Email { get; init; }
}

public record UserViewModel : ItemDetailBaseViewModel
{
	public required UserDTO User { get; init; }
}
