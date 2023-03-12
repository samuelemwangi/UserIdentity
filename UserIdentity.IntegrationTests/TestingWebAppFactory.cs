using System.Linq;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using UserIdentity.Persistence;

namespace UserIdentity.IntegrationTests
{
	public class TestingWebAppFactory : WebApplicationFactory<Program>
	{
		protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
		{
			webHostBuilder.ConfigureServices(services =>
			{
				var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

				if (descriptor != null)
					services.Remove(descriptor);

				services.AddDbContext<AppDbContext>(options =>
				{
					options.UseInMemoryDatabase("InMemTest");
				});

				var sp = services.BuildServiceProvider();

				using var scope = sp.CreateScope();

				using var appContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

				appContext.Database.EnsureCreated();

			});			

		}
	}
}
