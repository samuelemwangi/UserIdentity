using System;
using System.IO;

using Microsoft.Extensions.Configuration;

namespace UserIdentity.UnitTests;

public class TestSettingsFixture : IDisposable
{
  public IConfiguration Configuration { get; internal set; }

  public TestSettingsFixture()
  {
    Configuration = LoadTestConfiguration();
  }

  public void SetConfiguration()
  {
    Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
  }

  private static IConfiguration LoadTestConfiguration()
  {
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables()
        .Build();
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
  }
}
