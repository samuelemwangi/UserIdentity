using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

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
        private readonly JWKSController _jWKSController;

        public JWKSControllerTests(TestSettingsFixture testSettings)
        {
            _testSettings = testSettings;
            _keySetFactory = new KeySetFactory(_testSettings.Configuration);
            _jWKSController = new JWKSController(new GetKeySetsQueryHandler(_keySetFactory));

        }

        [Fact]
        public async Task Get_KeySets_Returns_KeySets()
        {
            // Arrange
            var actionResult = await _jWKSController.GetKeySetsAsync();

            // Act
            var result = actionResult as OkObjectResult;
            var kvp = result?.Value as Dictionary<string, IList<Dictionary<string, string>>>;


            // Assert
            Assert.NotNull(result);
            Assert.IsType<Dictionary<string, IList<Dictionary<string, string>>>>(result?.Value);
            Assert.NotNull(kvp?["keys"]);

            Assert.Equal(kvp?["keys"]?[0]["alg"], _testSettings.Configuration.GetSection("KeySetOptions")["Alg"]);
            Assert.Equal(kvp?["keys"]?[0]["kty"], _testSettings.Configuration.GetSection("KeySetOptions")["KeyType"]);
        }
    }
}

