using PolyzenKit.Application.Core;

using UserIdentity.Domain.Identity;

namespace UserIdentity.Application.Core.RegisteredApps.ViewModels;

public record RegisteredAppsViewModel : ItemsBaseViewModel
{
	public List<RegisteredAppDTO> RegisteredApps { get; init; } = null!;
}
