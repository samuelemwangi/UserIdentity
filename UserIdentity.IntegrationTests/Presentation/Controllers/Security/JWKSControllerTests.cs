﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using UserIdentity.IntegrationTests.Presentation.Helpers;

using Xunit;
using Xunit.Abstractions;

namespace UserIdentity.IntegrationTests.Presentation.Controllers.Security;


public class JWKSControllerTests(
	TestingWebAppFactory testingWebAppFactory,
	ITestOutputHelper outputHelper
	) : BaseControllerTests(testingWebAppFactory, outputHelper)
{

	private readonly static string _baseUri = "/api/v1/JWKS/keys";

	[Fact]
	public async Task Get_KeySets_Returns_KeySets()
	{
		// Arrange & Act
		var response = await _httpClient.SendNoAuthRequestAsync(HttpMethod.Get, _baseUri);

		// Assert
		var responseString = await response.ValidateRequestResponseAsync();

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var jsonObject = SerDe.Deserialize<JObject>(responseString);

		Assert.NotNull(jsonObject);

		var keySets = jsonObject?.ToObject<IDictionary<string, IList<Dictionary<string, string>>>>();

		Assert.NotNull(keySets);

		Assert.Equal(1, keySets?.Count);

		var keysetList = keySets?["keys"];
		Assert.NotNull(keysetList);
		Assert.Equal(1, keysetList?.Count);

		Assert.Equal(5, keysetList?[0].Count);

		Assert.Equal("EdDSA", keysetList?[0]["alg"]);
		Assert.Equal("OKP", keysetList?[0]["kty"]);
		Assert.Equal("Ed25519", keysetList?[0]["crv"]);
		Assert.NotNull(keysetList?[0]["x"]);
		Assert.NotNull(keysetList?[0]["kid"]);
	}

}
