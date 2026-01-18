using PolyzenKit.Application.Core;

using UserIdentity.Domain.Users;

namespace UserIdentity.Application.Core.Tokens.ViewModels;

public record AccessTokenViewModel : BaseViewModel
{
  public AccessTokenDTO? AccessToken { get; init; }

  public string? RefreshToken { get; init; }
}
