using PolyzenKit.Application.Core;

using UserIdentity.Application.Core.Tokens.ViewModels;

namespace UserIdentity.Application.Core.Users.ViewModels;

public record AuthUserViewModel : ItemDetailBaseViewModel
{
    public UserDTO User { get; init; } = null!;
    public bool IsConfirmed { get; init; }
    public AccessTokenViewModel? UserToken { get; init; }
}
