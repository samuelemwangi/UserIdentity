namespace UserIdentity.Application.Core.Roles.ViewModels
{
	public record RoleClaimDTO
	{
		public string Resource { get; init; }
		public string Action { get; init; }

		public string Scope { get; init; }
	}
	public record RoleClaimViewModel : ItemDetailBaseViewModel
	{
		public RoleClaimDTO RoleClaim { get; init; }
	}
}
