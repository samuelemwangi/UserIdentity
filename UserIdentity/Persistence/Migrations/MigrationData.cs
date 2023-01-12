using Microsoft.EntityFrameworkCore;
using UserIdentity.Persistence;

namespace UserIdentity.Persistence.Migrations
{
	public static class MigrationData
	{
		public static void MigrateDb(IApplicationBuilder app)
		{
			using var serviceScope = app.ApplicationServices.CreateScope();
			var appDbContext = serviceScope.ServiceProvider.GetService<AppDbContext>()!;

			MigrateUsers(appDbContext);
		}

		public static void MigrateUsers(AppDbContext appDbContext)
		{
			appDbContext.Database.EnsureCreated();

			// Avoid migrating when running in mem db (for tests)
			if (appDbContext.Database.IsRelational())
				appDbContext.Database.Migrate();
		}
	}
}
