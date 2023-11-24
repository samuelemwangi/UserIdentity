namespace UserIdentity.Application.Core.Roles.ViewModels
{
	public record RoleDTO
	{
		public String Id { get; init; }
		public String Name { get; init; }
	}
	public record RoleViewModel : ItemDetailBaseViewModel
	{
		public RoleDTO Role { get; init; }
	}
}
