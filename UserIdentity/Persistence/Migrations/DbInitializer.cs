using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Core.RegisteredApps.Commands;
using PolyzenKit.Application.Core.RegisteredApps.ViewModels;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Common.Utilities;
using PolyzenKit.Presentation.Settings;
using UserIdentity.Application.Core.RegisteredApps.Settings;

namespace UserIdentity.Persistence.Migrations;

public static class DbInitializer
{
  public static void InitializeDb(IApplicationBuilder app)
  {
    using var serviceScope = app.ApplicationServices.CreateScope();
    var appDbContext = serviceScope.ServiceProvider.GetService<AppDbContext>()!;
    var configuration = serviceScope.ServiceProvider.GetService<IConfiguration>()!;

    MigrateDb(appDbContext);
    SeedRolesData(serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>()!, serviceScope.ServiceProvider.GetService<IOptions<RoleSettings>>()!);
    SeedRegisteredAppsData(appDbContext, configuration, serviceScope.ServiceProvider.GetService<ICreateItemCommandHandler<CreateRegisteredAppCommand, RegisteredAppViewModel>>()!);
  }

  public static void MigrateDb(AppDbContext appDbContext)
  {
    if (appDbContext.Database.IsRelational())
      appDbContext.Database.Migrate();

    else
      appDbContext.Database.EnsureCreated();
  }

  #region Seed Data
  private static void SeedRolesData(RoleManager<IdentityRole> roleManager, IOptions<RoleSettings> roleSettingsOptions)
  {
    var roleSettings = roleSettingsOptions.Value;
    var seedRolesList = roleSettings.AdminRoles.Split(ZenConstants.LIST_CONFIG_SEPARATOR).Select(r => $"{roleSettings.RolePrefix}{r.Trim()}").ToHashSet();
    seedRolesList.Add($"{roleSettings.RolePrefix}{roleSettings.DefaultRole.Trim()}");

    foreach (var role in seedRolesList)
    {
      // check if role exists
      if (roleManager.FindByNameAsync(role).Result != null)
        continue;

      var identityRole = new IdentityRole
      {
        Name = role,
        NormalizedName = role.ToUpper(),
      };
      roleManager.CreateAsync(identityRole).Wait();
    }
  }

  private static void SeedRegisteredAppsData(AppDbContext appDbContext, IConfiguration configuration, ICreateItemCommandHandler<CreateRegisteredAppCommand, RegisteredAppViewModel> createItemCommandHandler)
  {
    var registeredAppSettings = configuration.GetSection(nameof(RegisteredAppsSettings)).Get<RegisteredAppsSettings>() ?? throw new MissingConfigurationException(nameof(RegisteredAppsSettings));

    foreach (var app in registeredAppSettings.RegisteredApps)
      if (!appDbContext.RegisteredApp.Any(ra => ra.AppName == app.AppName))
      {
        var createCommand = new CreateRegisteredAppCommand
        {
          AppName = app.AppName,
          AppSecretKey = app.AppSecretKey,
          CallbackUrl = app.CallbackUrl,
          CallbackHeaders = app.CallbackHeaders,
          ForwardServiceToken = app.ForwardServiceToken
        };

        _ = createItemCommandHandler.CreateItemAsync(createCommand, ZenConstants.SYSTEM_USER_ID).Result;
      }
  }
  #endregion
}
