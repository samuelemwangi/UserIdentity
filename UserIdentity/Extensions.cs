using System.Reflection;

using PolyzenKit;
using PolyzenKit.Application.Core.Errors.Queries.GerError;
using PolyzenKit.Persistence.Repositories.AppEntities;

namespace UserIdentity;

public static class Extensions
{
	// Add App Command and Query Handlers
	public static void AddAppCommandAndQueryHandlers(this IServiceCollection services)
	{
		services.AddAppCommandAndQueryHandlers(Assembly.GetExecutingAssembly());

		var polyzenKitAssembly = Assembly.GetAssembly(typeof(GetErrorQueryHandler))!;

		services.AddAppCommandAndQueryHandlers(polyzenKitAssembly);
	}

	// Add Repositories
	public static void AddAppRepositories(this IServiceCollection services)
	{
		services.AddAppRepositories(Assembly.GetExecutingAssembly());

		var polyzenKitAssembly = Assembly.GetAssembly(typeof(AppEntityRepository))!;
		services.AddAppRepositories(polyzenKitAssembly);
	}
	

	// Seed Entity Names
	public static void AppSeedEntityNamesData(this IApplicationBuilder applicationBuilder)
	{
		Assembly.GetExecutingAssembly().AppSeedEntityNamesData(applicationBuilder);
	}
}
