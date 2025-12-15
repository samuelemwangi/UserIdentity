using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

using PolyzenKit.Common.Exceptions;
using PolyzenKit.Infrastructure.Security.KeySets;
using PolyzenKit.Persistence.Settings;

using Testcontainers.MySql;

using Xunit;

namespace UserIdentity.IntegrationTests;

public class TestingWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _mysqlContainer;
    private readonly Dictionary<string, string?> _environmentSnapshot = new(StringComparer.OrdinalIgnoreCase);
    private readonly KeySetOptions _keySetOptions;

    public TestingWebAppFactory()
    {
        var configuration = LoadTestConfiguration();

        var mysqlSettings = configuration.GetSection(nameof(MysqlSettings)).Get<MysqlSettings>()
            ?? throw new MissingConfigurationException(nameof(MysqlSettings));

        _keySetOptions = configuration.GetSection(nameof(KeySetOptions)).Get<KeySetOptions>()
            ?? throw new MissingConfigurationException(nameof(KeySetOptions));

        _mysqlContainer = new MySqlBuilder()
            .WithImage("mysql:8.4.0")
            .WithUsername(mysqlSettings.UserName)
            .WithPassword(mysqlSettings.Password)
            .WithDatabase(mysqlSettings.Database)
            .WithPortBinding(int.Parse(mysqlSettings.Port), 3306)
            .Build();
    }

    private static IConfiguration LoadTestConfiguration()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        configBuilder.AddEnvironmentVariables();

        return configBuilder.Build();
    }


    public async Task InitializeAsync()
    {
        await _mysqlContainer.StartAsync();

        var keyPairGenerator = new Ed25519KeyPairGenerator();
        keyPairGenerator.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
        var keyPair = keyPairGenerator.GenerateKeyPair();

        SetTemporaryEnvironmentVariable(_keySetOptions.PrivateKeyPath!, ConvertToPem(keyPair.Private, _keySetOptions.PrivateKeyPassPhrase));
        SetTemporaryEnvironmentVariable(_keySetOptions.PublicKeyPath, ConvertToPem(keyPair.Public));
    }

    public new async Task DisposeAsync()
    {
        RestoreEnvironmentVariables();
        await base.DisposeAsync();
        await _mysqlContainer.DisposeAsync();
    }

    private void SetTemporaryEnvironmentVariable(string key, string? value)
    {
        if (!_environmentSnapshot.ContainsKey(key))
        {
            _environmentSnapshot[key] = Environment.GetEnvironmentVariable(key);
        }

        Environment.SetEnvironmentVariable(key, value);
    }

    private void RestoreEnvironmentVariables()
    {
        foreach (var kvp in _environmentSnapshot)
        {
            Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
        }

        _environmentSnapshot.Clear();
    }

    private static string ConvertToPem(AsymmetricKeyParameter key, string? passphrase = null)
    {
        using var stringWriter = new StringWriter();
        var pemWriter = new PemWriter(stringWriter);
        if (passphrase != null)
        {
            var encryptor = new Pkcs8Generator(key, Pkcs8Generator.PbeSha1_3DES)
            {
                Password = passphrase.ToCharArray()
            };
            pemWriter.WriteObject(encryptor);
        }
        else
        {
            pemWriter.WriteObject(key);
        }

        pemWriter.Writer.Flush();
        return stringWriter.ToString();
    }
}
