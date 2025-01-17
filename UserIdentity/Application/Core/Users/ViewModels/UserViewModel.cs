using PolyzenKit.Application.Core;
using PolyzenKit.Domain.DTO;

namespace UserIdentity.Application.Core.Users.ViewModels
{
	public record UserDTO : BaseEntityDTO<string>
	{
		public string? FullName { get; init; }

		public string? UserName { get; init; }

		public string? Email { get; init; }
	}

	public record UserViewModel : ItemDetailBaseViewModel
	{
		public required UserDTO User { get; init; }
	}
}
