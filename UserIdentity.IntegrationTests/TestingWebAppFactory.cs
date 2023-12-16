using System;
using System.Collections.Generic;
using System.IO;
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

		public Dictionary<string, string> Props { get; internal set; }
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

		public Dictionary<string, string> GetProps()
		{
			Dictionary<string, string> props = new Dictionary<string, string>();
			string filePath = ".env";
			if (!File.Exists(filePath))
				return props;


			foreach (string line in File.ReadLines(filePath))
			{
				string[] parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
				props.Add(parts[0].Trim(), parts[1].Trim());
			}

			return props;
		}
	}
}
