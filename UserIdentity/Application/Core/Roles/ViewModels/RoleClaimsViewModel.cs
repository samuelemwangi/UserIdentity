using PolyzenKit.Application.Core;

namespace UserIdentity.Application.Core.Roles.ViewModels
{
	public record RoleClaimsViewModel : ItemsBaseViewModel
	{
		public ICollection<RoleClaimDTO> RoleClaims { get; init; } = null!;
	}
}
