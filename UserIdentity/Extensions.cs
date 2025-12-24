using System.Reflection;

using Microsoft.EntityFrameworkCore;

using PolyzenKit;
using PolyzenKit.Application.Core.Errors.Queries.GerError;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Persistence.Repositories.AppEntities;
using PolyzenKit.Persistence.Settings;

using UserIdentity.Application.Core.Users.Settings;
using UserIdentity.Application.Interfaces;
using UserIdentity.Infrastructure.ExternalServices;

namespace UserIdentity;

public static class Extensions
{
  // Add Mysql
  public static void AddAppMysql<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
  {
    var mysqlSettings = configuration.GetSection(nameof(MysqlSettings)).Get<MysqlSettings>() ?? throw new MissingConfigurationException(nameof(MysqlSettings));

    services.AddDbContext<DbContext, TContext>(options =>
    {
      options.UseMySql(mysqlSettings.ConnectionString, ServerVersion.AutoDetect(mysqlSettings.ConnectionString)).UseSnakeCaseNamingConvention();
    });
  }

  // Add App Command and Query Handlers
  public static void AddAppCommandAndQueryHandlers(this IServiceCollection services)
  {
    services.AddAppCommandAndQueryHandlers(Assembly.GetExecutingAssembly());

    var polyzenKitAssembly = Assembly.GetAssembly(typeof(GetErrorQueryHandler))!;

    services.AddAppCommandAndQueryHandlers(polyzenKitAssembly);
  }

  // Add App Event Handlers
  public static void AddAppEventHandlers(this IServiceCollection services)
  {
    services.AddAppEventHandlers(Assembly.GetExecutingAssembly());
    var polyzenKitAssembly = Assembly.GetAssembly(typeof(GetErrorQueryHandler))!;
    services.AddAppEventHandlers(polyzenKitAssembly);
  }

  // Add Repositories
  public static void AddAppRepositories(this IServiceCollection services)
  {
    services.AddAppRepositories(Assembly.GetExecutingAssembly());

    var polyzenKitAssembly = Assembly.GetAssembly(typeof(AppEntityRepository))!;
    services.AddAppRepositories(polyzenKitAssembly);
  }

  // Add Google Recaptcha
  public static void AddGoogleRecaptcha(this IServiceCollection services, IConfiguration configuration)
  {
    var googleRecaptchaSettings = configuration.GetSection(nameof(GoogleRecaptchaSettings)).Get<GoogleRecaptchaSettings>() ?? throw new MissingConfigurationException(nameof(GoogleRecaptchaSettings));

    if (googleRecaptchaSettings.Enabled && googleRecaptchaSettings.SiteKey == null)
      throw new MissingConfigurationException(nameof(GoogleRecaptchaSettings.SiteKey));

    services.Configure<GoogleRecaptchaSettings>(options =>
    {
      options.Enabled = googleRecaptchaSettings.Enabled;
      options.SiteKey = googleRecaptchaSettings.SiteKey;
    });

    services.AddScoped<IGoogleRecaptchaService, GoogleRecaptchaService>();
  }

  public static void AddAppKafka(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddAppKafka(configuration, Assembly.GetExecutingAssembly());
  }


  // Seed Entity Names
  public static void AppSeedEntityNamesData(this IApplicationBuilder applicationBuilder)
  {
    Assembly.GetExecutingAssembly().AppSeedEntityNamesData(applicationBuilder);
  }
}
