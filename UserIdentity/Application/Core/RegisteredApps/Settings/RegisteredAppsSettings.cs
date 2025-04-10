using UserIdentity.Domain.Identity;

namespace UserIdentity.Application.Core.RegisteredApps.Settings;

public record RegisteredAppsSettings
{
  public List<RegisteredAppEntity> RegisteredApps { get; init; } = [];
}

