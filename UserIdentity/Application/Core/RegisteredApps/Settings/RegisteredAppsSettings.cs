using PolyzenKit.Domain.RegisteredApps;

namespace UserIdentity.Application.Core.RegisteredApps.Settings;

public record RegisteredAppsSettings
{
  public List<RegisteredAppEntity> RegisteredApps { get; init; } = [];
}

