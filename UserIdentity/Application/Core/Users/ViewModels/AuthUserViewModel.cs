using UserIdentity.Application.Core.Tokens.ViewModels;

namespace UserIdentity.Application.Core.Users.ViewModels
{
	public record AuthUserViewModel : ItemDetailBaseViewModel
	{
		public UserDTO? UserDetails { get; init; }
		public AccessTokenViewModel? UserToken { get; init; }
	}
}
