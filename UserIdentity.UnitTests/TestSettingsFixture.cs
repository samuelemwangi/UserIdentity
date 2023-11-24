using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Configuration;

namespace UserIdentity.UnitTests
{
	public class TestSettingsFixture : IDisposable
	{
		public IConfiguration Configuration { get; internal set; }
		public Dictionary<String, String> Props { get; internal set; }

		public TestSettingsFixture()
		{
			Props = GetProps();

			foreach (var prop in Props)
				Environment.SetEnvironmentVariable(prop.Key, prop.Value + "");

			SetConfiguration();

		}

		public void SetConfiguration()
		{
			Configuration = new ConfigurationBuilder()
					.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
					.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
					.AddEnvironmentVariables()
					.Build();
		}

		public Dictionary<String, String> GetProps()
		{
			Dictionary<String, String> props = new Dictionary<String, String>();
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

		public void Dispose()
		{
		}
	}
}
