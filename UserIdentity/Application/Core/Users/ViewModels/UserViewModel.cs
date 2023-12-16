namespace UserIdentity.Application.Core.Users.ViewModels
{
	public record UserDTO : BaseEntityDTO
	{
		public new string? Id { get; init; }
		public string? FullName { get; init; }
		public string? UserName { get; init; }
		public string? Email { get; init; }

	}

	public record UserViewModel : ItemDetailBaseViewModel
	{
		public UserDTO? User { get; init; }
	}
}
