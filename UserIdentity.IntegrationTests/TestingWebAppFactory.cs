using System.Collections.Generic;
using System;
using System.Linq;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using UserIdentity.Persistence;
using System.IO;

namespace UserIdentity.IntegrationTests
{
	public class TestingWebAppFactory : WebApplicationFactory<Program>
	{

		public Dictionary<String, String> Props { get; internal set; }
		public TestingWebAppFactory()
		{
			Props = GetProps();

			foreach (var prop in Props)
				Environment.SetEnvironmentVariable(prop.Key, prop.Value + "");
		}
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

		public Dictionary<String, String> GetProps()
		{
			Dictionary<String, String> props = new Dictionary<string, string>();
			String filePath = ".env";
			if (!File.Exists(filePath))
				return props;


			foreach (String line in File.ReadLines(filePath))
			{
				String[] parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
				props.Add(parts[0].Trim(), parts[1].Trim());
			}

			return props;
		}
	}
}
