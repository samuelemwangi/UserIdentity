namespace UserIdentity.Application.Core.Roles.ViewModels
{
	public record RoleDTO
	{
		public string Id { get; init; }
		public string Name { get; init; }
	}
	public record RoleViewModel : ItemDetailBaseViewModel
	{
		public RoleDTO Role { get; init; }
	}
}
