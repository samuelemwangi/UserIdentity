using System.Reflection;

using Microsoft.AspNetCore.Identity;

using PolyzenKit.Application.Core.Errors.Queries.GerError;
using PolyzenKit.Persistence.Repositories.AppEntities;
using PolyzenKit.Presentation;

using UserIdentity.Persistence;

namespace UserIdentity
{
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

		// Add Identity 
		public static void AddAppIdentity(this IServiceCollection services)
		{

			// add identity
			var identityBuilder = services.AddIdentityCore<IdentityUser>(o =>
			{
				// configure identity options
				o.Password.RequireDigit = true;
				o.Password.RequireLowercase = false;
				o.Password.RequireUppercase = false;
				o.Password.RequireNonAlphanumeric = false;
				o.Password.RequiredLength = 4;
			});

			identityBuilder.AddRoles<IdentityRole>();

			identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole), identityBuilder.Services);
			identityBuilder.AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
		}

		// Seed Entity Names
		public static void AppSeedEntityNamesData(this IApplicationBuilder applicationBuilder)
		{
			Assembly.GetExecutingAssembly().AppSeedEntityNamesData(applicationBuilder);
		}
	}
}
