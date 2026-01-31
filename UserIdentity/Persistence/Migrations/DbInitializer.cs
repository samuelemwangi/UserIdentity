using Microsoft.EntityFrameworkCore;

namespace UserIdentity.Persistence.Migrations;

public static class DbInitializer
{
  public static void InitializeDb(IApplicationBuilder app)
  {
    using var serviceScope = app.ApplicationServices.CreateScope();
    var appDbContext = serviceScope.ServiceProvider.GetService<AppDbContext>()!;
    var configuration = serviceScope.ServiceProvider.GetService<IConfiguration>()!;

    MigrateDb(appDbContext);
  }

  public static void MigrateDb(AppDbContext appDbContext)
  {
    if (appDbContext.Database.IsRelational())
      appDbContext.Database.Migrate();

    else
      appDbContext.Database.EnsureCreated();
  }
}
