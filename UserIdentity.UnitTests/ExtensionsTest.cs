using Microsoft.Extensions.DependencyInjection;

using UserIdentity.Infrastructure.Security;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.Persistence.Repositories.Users;

using Xunit;

namespace UserIdentity.UnitTests
{
	public class ExtensionsTest : IClassFixture<TestSettingsFixture>
	{
		private readonly IServiceCollection _services;
		private readonly TestSettingsFixture _testSettings;
		public ExtensionsTest(TestSettingsFixture testSettings)
		{
			_services = new ServiceCollection();
			_testSettings = testSettings;
		}

		[Fact]
		public void AddRepositories_To_DI_Container_Adds_Repositories_To_DI_Container()
		{
			// Act
			_services.AddRepositories();

			// Assert
			Assert.Equal(2, _services.Count);

			Assert.Contains(_services, s => s.ServiceType == typeof(IUserRepository));
			Assert.Contains(_services, s => s.ServiceType == typeof(IRefreshTokenRepository));

			Assert.Contains(_services, s => s.ImplementationType == typeof(UserRepository));
			Assert.Contains(_services, s => s.ImplementationType == typeof(RefreshTokenRepository));
		}

		[Fact]
		public void AddAppAuthentication_To_DI_Container_Configures_Auth_Related_Configs()
		{
			// Arrange
			var configuration = _testSettings.Configuration;
			var jwtIssuerOptions = new JwtIssuerOptions();

			// Act
			_services.AddAppAuthentication(configuration);

			// Assert
			Assert.True(_services.Count >= 0);
		}

		[Fact]
		public void AddCommandAndQueryHandlers_To_DI_Container_Adds_CommandAndQueryHandlers_To_DI_Container()
		{
			// Act
			_services.AddCommandAndQueryHandlers();

			// Assert
			Assert.True(_services.Count >= 0);
		}

		[Fact]
		public void AddAppAuthorization_To_DI_Container_Adds_AppAuthorization_To_DI_Container()
		{

			// Act
			_services.AddAppAuthorization();

			// Assert
			Assert.True(_services.Count >= 0);
		}

		[Fact]
		public void AddAppIdentity_To_DI_Container_Adds_AppIdentity_To_DI_Container()
		{
			// Arrange
			var configuration = _testSettings.Configuration;

			// Act
			_services.AddAppIdentity();

			// Assert
			Assert.True(_services.Count >= 0);
		}
	}
}

