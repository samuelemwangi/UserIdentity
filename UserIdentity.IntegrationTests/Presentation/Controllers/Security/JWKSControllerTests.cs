﻿using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UserIdentity.IntegrationTests.Presentation.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace UserIdentity.IntegrationTests.Presentation.Controllers.Security
{

  public class JWKSControllerTests : BaseControllerTests
  {

    private readonly static String _baseUri = "/api/v1/JWKS/keys";

    public JWKSControllerTests(TestingWebAppFactory testingWebAppFactory, ITestOutputHelper outputHelper)
        : base(testingWebAppFactory, outputHelper)
    {
    }

    [Fact]
    public async Task Get_KeySets_Returns_KeySets()
    {
      // Arrange
      var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri);

      // Act
      var response = await _httpClient.SendAsync(httpRequest);
      var responseString = await response.Content.ReadAsStringAsync();

      // Assert
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);

      var jsonObject = SerDe.Deserialize<JObject>(responseString);

      Assert.NotNull(jsonObject);

      var keySets = jsonObject?.ToObject<IDictionary<String, IList<Dictionary<String, String>>>>();

      Assert.NotNull(keySets);

      Assert.Equal(1, keySets?.Count);

      var keysetList = keySets?["keys"];
      Assert.NotNull(keysetList);
      Assert.Equal(1, keysetList?.Count);

      Assert.Equal(4, keysetList?[0].Count);

      Assert.Equal("HS256", keysetList?[0]["alg"]);
      Assert.Equal("oct", keysetList?[0]["kty"]);
      Assert.Equal(Base64UrlEncoder.Encode(_props["APP_KEY_ID"]), keysetList?[0]["kid"]);
      Assert.Equal(Base64UrlEncoder.Encode(_props["APP_SECRET_KEY"]), keysetList?[0]["k"]);
    }

  }
}
