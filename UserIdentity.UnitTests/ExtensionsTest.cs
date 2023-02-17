using System;
using Microsoft.Extensions.DependencyInjection;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.Persistence.Repositories.Users;
using Xunit;
using FakeItEasy;
using UserIdentity.Persistence;
using Microsoft.Extensions.Configuration;
using UserIdentity.Infrastructure.Security;

namespace UserIdentity.UnitTests
{
	public class ExtensionsTest: IClassFixture<TestSettingsFixture>
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
	}
}

