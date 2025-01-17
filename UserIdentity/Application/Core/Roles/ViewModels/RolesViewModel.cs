using PolyzenKit.Application.Core;

namespace UserIdentity.Application.Core.Roles.ViewModels
{

	public record RolesViewModel : ItemsBaseViewModel
	{
		public ICollection<RoleDTO> Roles { get; init; } = null!;
	}

	public record UserRolesViewModel : ItemsBaseViewModel
	{
		public ICollection<string> UserRoles { get; init; } = null!;
	}
}
