using System;
using System.IO;
using System.Threading;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using MySqlConnector;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

using PolyzenKit.Infrastructure.Security.KeySets;
using PolyzenKit.Presentation.Settings;

using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

using Testcontainers.MySql;

using UserIdentity.IntegrationTests.TestUtils;
using UserIdentity.Persistence;

namespace UserIdentity.IntegrationTests;

public class TestingWebAppFactory : WebApplicationFactory<Program>
{
    private readonly MySqlContainer _mysqlContainer;
    private readonly string _connectionString;

    public TestingWebAppFactory()
    {
        _mysqlContainer = new MySqlBuilder()
            .WithImage("mysql:8.4.0")
            .WithUsername("testuser")
            .WithPassword("Test@123")
            .WithDatabase("useridentity_test")
            .Build();

        _mysqlContainer.StartAsync().GetAwaiter().GetResult();

        var mappedPort = _mysqlContainer.GetMappedPublicPort(3306);

        var connectionStringBuilder = new MySqlConnectionStringBuilder(_mysqlContainer.GetConnectionString())
        {
            AllowPublicKeyRetrieval = true,
            SslMode = MySqlSslMode.None,
            Server = "127.0.0.1",
            Port = (uint)mappedPort
        };

        _connectionString = connectionStringBuilder.ConnectionString;

        // Ensure the app-level MySQL settings point to the container's mapped port
        Environment.SetEnvironmentVariable("MysqlSettings__Host", "127.0.0.1");
        Environment.SetEnvironmentVariable("MysqlSettings__Port", mappedPort.ToString());
        Environment.SetEnvironmentVariable("MysqlSettings__Database", "useridentity_test");
        Environment.SetEnvironmentVariable("MysqlSettings__UserName", "testuser");
        Environment.SetEnvironmentVariable("MysqlSettings__Password", "Test@123");

        Console.WriteLine($"[TestingWebAppFactory] MySQL mapped port: {mappedPort}, connection string: {_connectionString}");

        var privateKeyFilename = "privateKey.pem";
        var publicKeyFilename = "publicKey.pem";

        var privateKeyPath = Path.Combine(AppContext.BaseDirectory, privateKeyFilename);
        var publicKeyPath = Path.Combine(AppContext.BaseDirectory, publicKeyFilename);

        var keyPairGenerator = new Ed25519KeyPairGenerator();
        keyPairGenerator.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
        var keyPair = keyPairGenerator.GenerateKeyPair();

        var privateKey = keyPair.Private;
        var publicKey = keyPair.Public;

        var passphrase = "aG00dPassPhr4a2e";
        var privateKeyPem = ConvertToPem(privateKey, passphrase);
        var publicKeyPem = ConvertToPem(publicKey);

        File.WriteAllText(privateKeyPath, privateKeyPem);
        File.WriteAllText(publicKeyPath, publicKeyPem);

        Environment.SetEnvironmentVariable($"{nameof(KeySetOptions)}__{nameof(KeySetOptions.PrivateKeyPath)}", privateKeyFilename);
        Environment.SetEnvironmentVariable($"{nameof(KeySetOptions)}__{nameof(KeySetOptions.PrivateKeyPassPhrase)}", passphrase);
        Environment.SetEnvironmentVariable($"{nameof(KeySetOptions)}__{nameof(KeySetOptions.PublicKeyPath)}", publicKeyFilename);

        Environment.SetEnvironmentVariable($"{nameof(ApiKeySettings)}__{nameof(ApiKeySettings.ApiKey)}", TestConstants.ApiKey);

        Environment.SetEnvironmentVariable($"{nameof(RoleSettings)}__{nameof(RoleSettings.DefaultRole)}", ApiRoleSettings.DefaultRoleBase);
        Environment.SetEnvironmentVariable($"{nameof(RoleSettings)}__{nameof(RoleSettings.AdminRoles)}", ApiRoleSettings.AdminRolesBase);
        Environment.SetEnvironmentVariable($"{nameof(RoleSettings)}__{nameof(RoleSettings.ServiceName)}", ApiRoleSettings.ServiceName);
    }

    protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
    {

        webHostBuilder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.RemoveAll(typeof(IDbContextFactory<AppDbContext>));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(
                    _connectionString,
                    ServerVersion.Create(new Version(8, 0, 0), ServerType.MySql),
                    mysqlOptions =>
                    {
                        mysqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(2), null);
                    });
            });

            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            const int maxAttempts = 5;
            var retryDelay = TimeSpan.FromSeconds(2);

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    appContext.Database.Migrate();
                    break;
                }
                catch (MySqlException) when (attempt < maxAttempts)
                {
                    Thread.Sleep(retryDelay);
                }
            }
        });

    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _mysqlContainer.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }

    private string ConvertToPem(AsymmetricKeyParameter key, string? passphrase = null)
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
