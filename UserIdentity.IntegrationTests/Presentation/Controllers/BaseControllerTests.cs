using System;
using System.Net.Http;

using Microsoft.AspNetCore.Identity;

using UserIdentity.IntegrationTests.Persistence;
using UserIdentity.Persistence;

using Xunit;
using Xunit.Abstractions;

namespace UserIdentity.IntegrationTests.Presentation.Controllers
{
	public class BaseControllerTests : IClassFixture<TestingWebAppFactory>, IDisposable
	{

		protected readonly ITestOutputHelper _outputHelper;

		protected readonly HttpClient _httpClient;
		protected readonly IServiceProvider _serviceProvider;

		protected readonly AppDbContext _appDbContext;
		protected readonly UserManager<IdentityUser> _userManager;

		public BaseControllerTests(TestingWebAppFactory testingWebAppFactory, ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;

			_httpClient = testingWebAppFactory.CreateClient();
			_serviceProvider = testingWebAppFactory.Services;

			_appDbContext = ServiceResolver.ResolveService<AppDbContext>(_serviceProvider);
			_userManager = ServiceResolver.ResolveService<UserManager<IdentityUser>>(_serviceProvider);

			SetUp();
		}

		public void SetUp()
		{
			DBContexUtils.ClearDatabase(_appDbContext);
		}


		public void Dispose()
		{
			_appDbContext.Dispose();
			_userManager.Dispose();
		}
	}
}
