using PolyzenKit.Application.Core;

using UserIdentity.Domain.Identity;

namespace UserIdentity.Application.Core.RegisteredApps.ViewModels;

public record RegisteredAppViewModel : ItemDetailBaseViewModel
{
	public RegisteredAppDTO RegisteredApp { get; init; } = null!;
}
