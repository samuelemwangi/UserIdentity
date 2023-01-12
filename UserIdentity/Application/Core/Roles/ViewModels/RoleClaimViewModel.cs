namespace UserIdentity.Application.Core.Roles.ViewModels
{
	public record RoleClaimDTO
	{
		public String Resource { get; init; }
		public String Action { get; init; }

		public String Scope { get; init; }
	}
	public record RoleClaimViewModel: ItemDetailBaseViewModel
	{
		public RoleClaimDTO RoleClaim { get; init; }
	}
}
