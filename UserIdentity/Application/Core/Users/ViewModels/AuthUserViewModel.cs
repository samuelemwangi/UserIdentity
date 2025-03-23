using PolyzenKit.Application.Core;

using UserIdentity.Application.Core.Tokens.ViewModels;

namespace UserIdentity.Application.Core.Users.ViewModels;

public record AuthUserViewModel : ItemDetailBaseViewModel
{
	public UserDTO UserDetails { get; init; } = null!;

	public AccessTokenViewModel UserToken { get; init; } = null!;
}
