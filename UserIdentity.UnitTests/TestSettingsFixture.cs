using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace UserIdentity.UnitTests
{
    public class TestSettingsFixture : IDisposable
    {
        public IConfiguration Configuration { get; init; }
        private static int heats =  0;

        public TestSettingsFixture()
        {

            foreach (var prop in GetProps())
                Environment.SetEnvironmentVariable(prop.Key, prop.Value + "");

            Configuration = SetConfiguration();

        }

        public IConfiguration SetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public Dictionary<String, String> GetProps()
        {
            Console.WriteLine("---------");
            Console.WriteLine("::" + ++heats + "::");
            Console.WriteLine("---------");
            Dictionary<String, String> props = new Dictionary<string, string>();
            String filePath = ".env";
            if (!File.Exists(filePath))
                return props;

            
            foreach (String line in File.ReadLines(filePath))
            {
                String[] parts =  line.Split('=', StringSplitOptions.RemoveEmptyEntries);
                props.Add(parts[0].Trim(), parts[1].Trim());
            }
            
            return props;
        }

        public void Dispose()
        {
        }
    }
}