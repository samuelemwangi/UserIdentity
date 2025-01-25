using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using PolyzenKit.Common.Utilities;
using PolyzenKit.Presentation.Settings;

namespace UserIdentity.Persistence.Migrations
{
	public static class DbInitializer
	{
		public static void InitializeDb(IApplicationBuilder app)
		{
			using var serviceScope = app.ApplicationServices.CreateScope();
			var appDbContext = serviceScope.ServiceProvider.GetService<AppDbContext>()!;

			MigrateDb(appDbContext);
			SeedRolesData(serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>()!, serviceScope.ServiceProvider.GetService<IOptions<RoleSettings>>()!);
		}

		public static void MigrateDb(AppDbContext appDbContext)
		{
			if (appDbContext.Database.IsRelational())
				appDbContext.Database.Migrate();

			else
				appDbContext.Database.EnsureCreated();
		}

		#region Seed Data
		private static void SeedRolesData(RoleManager<IdentityRole> roleManager, IOptions<RoleSettings> roleSettings)
		{
			var seedRolesList = roleSettings.Value.AdminRoles.Split(ZenConstants.LIST_CONFIG_SEPARATOR).Select(r => r.Trim()).ToHashSet();
			seedRolesList.Add(roleSettings.Value.DefaultRole.Trim());

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
		#endregion
	}
}
