using System;
using Microsoft.Extensions.DependencyInjection;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.Persistence.Repositories.Users;
using Xunit;
using FakeItEasy;
using UserIdentity.Persistence;

namespace UserIdentity.UnitTests
{
	public class ExtensionsTest
	{
		private readonly IServiceCollection _services;
		public ExtensionsTest()
		{
			_services = new ServiceCollection();
		}

		[Fact]
		public void AddRepositories_to_DI_Container_Adds_Repositories_To_DI_Container()
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
	}
}

