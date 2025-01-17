using System.Reflection;

using Microsoft.AspNetCore.Identity;

using PolyzenKit.Application.Core.Errors.Queries.GerError;
using PolyzenKit.Infrastructure.Security.KeyProviders;
using PolyzenKit.Presentation;

using UserIdentity.Persistence;

namespace UserIdentity
{
	public static class Extensions
	{
		public static void AddCommandAndQueryHandlers(this IServiceCollection services)
		{
			services.AddCommandAndQueryHandlers(Assembly.GetExecutingAssembly());

			var polyzenKitAssembly = Assembly.GetAssembly(typeof(GetErrorQueryHandler))!;

			services.AddCommandAndQueryHandlers(polyzenKitAssembly);
		}

		public static void AddRepositories(this IServiceCollection services)
		{
			services.AddRepositories(Assembly.GetExecutingAssembly());
		}

		public static IKeyProvider ResolveKeyProvider(IConfiguration configuration) => new FileSystemKeyProvider();

		public static void AddAppAuthorization(this IServiceCollection services)
		{
			// api user claim policy
			services.AddAuthorization();
		}

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
	}
}
