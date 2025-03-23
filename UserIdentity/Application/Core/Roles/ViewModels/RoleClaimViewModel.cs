using PolyzenKit.Application.Core;

namespace UserIdentity.Application.Core.Roles.ViewModels;

public record RoleClaimDTO
{
	public string Resource { get; init; } = null!;

	public string Action { get; init; } = null!;

	public string Scope { get; init; } = null!;
}
public record RoleClaimViewModel : ItemDetailBaseViewModel
{
	public RoleClaimDTO RoleClaim { get; init; } = null!;
}
