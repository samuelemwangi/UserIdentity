using System;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

using PolyzenKit.Infrastructure.Security.KeySets;
using PolyzenKit.Presentation.Settings;

using UserIdentity.IntegrationTests.TestUtils;
using UserIdentity.Persistence;

namespace UserIdentity.IntegrationTests;

public class TestingWebAppFactory : WebApplicationFactory<Program>
{
    public TestingWebAppFactory()
    {
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
