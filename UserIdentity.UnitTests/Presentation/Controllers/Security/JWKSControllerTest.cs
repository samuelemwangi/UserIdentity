using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using UserIdentity.Application.Core.KeySets.Queries.GetKeySets;
using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Infrastructure.Security;
using UserIdentity.Presentation.Controllers.Security;

using Xunit;

namespace UserIdentity.UnitTests.Presentation.Controllers.Security
{
	public class JWKSControllerTest : IClassFixture<TestSettingsFixture>
	{
		private readonly TestSettingsFixture _testSettings;
		private readonly IKeySetFactory _keySetFactory;
		private readonly JWKSController _jWKSController;

		public JWKSControllerTest(TestSettingsFixture testSettings)
		{
			_testSettings = testSettings;
			_keySetFactory = new KeySetFactory(_testSettings.Configuration);
			_jWKSController = new JWKSController(new GetKeySetsQueryHandler(_keySetFactory));

		}

		[Fact]
		public void Get_KeySets_Returns_KeySets()
		{
			// Arrange
			var actionResult = _jWKSController.GetKeySets();

			// Act
			var result = actionResult as OkObjectResult;
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

