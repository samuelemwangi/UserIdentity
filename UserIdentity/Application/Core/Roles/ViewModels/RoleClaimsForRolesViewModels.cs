using PolyzenKit.Application.Core;

namespace UserIdentity.Application.Core.Roles.ViewModels
{
	public record RoleClaimsForRolesViewModels : BaseViewModel
	{
		public HashSet<string> RoleClaims { get; init; } = null!;
	}
}
