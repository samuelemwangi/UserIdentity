using PolyzenKit.Application.Core;

namespace UserIdentity.Application.Core.Roles.ViewModels;

public record RoleDTO
{
	public string Id { get; init; } = null!;
	public string Name { get; init; } = null!;
}
public record RoleViewModel : ItemDetailBaseViewModel
{
	public RoleDTO Role { get; init; } = null!;
}
