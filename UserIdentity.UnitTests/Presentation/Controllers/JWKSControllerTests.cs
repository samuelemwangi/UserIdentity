using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.KeySets.Queries.GetKeySets;
using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Infrastructure.Security;
using UserIdentity.Presentation.Controllers.Security;

using Xunit;

namespace UserIdentity.UnitTests.Presentation.Controllers
{
	public class JWKSControllerTests : IClassFixture<TestSettingsFixture>
	{
		private readonly TestSettingsFixture _testSettings;
		private readonly IKeySetFactory _keySetFactory;
		private readonly IGetItemsQueryHandler<GetKeySetsQuery, IDictionary<String, IList<Dictionary<String, String>>>> _queryHandler;

		public JWKSControllerTests(TestSettingsFixture testSettings)
		{
			_testSettings = testSettings;
			_keySetFactory = new KeySetFactory(_testSettings.Configuration);

			_queryHandler = new GetKeySetsQueryHandler(_keySetFactory);

		}

		[Fact]
		public async Task Get_KeySets_Returns_KeySets()
		{
			// Arrange
			var JWKSController = new JWKSController(_queryHandler);
			var actionResult = await JWKSController.GetKeySetsAsync();

			// Act
			var result = actionResult as ObjectResult;
			var kvp = result?.Value as Dictionary<String, IList<Dictionary<String, String>>>;


			// Assert
			Assert.NotNull(result);
			Assert.IsType<Dictionary<String, IList<Dictionary<String, String>>>>(result?.Value);
			Assert.NotNull(kvp?["keys"]);

			Assert.Equal(kvp?["keys"]?[0]["alg"], _testSettings.Configuration.GetSection("KeySetOptions")["Alg"]);
			Assert.Equal(kvp?["keys"]?[0]["kty"], _testSettings.Configuration.GetSection("KeySetOptions")["KeyType"]);
		}
	}
}

