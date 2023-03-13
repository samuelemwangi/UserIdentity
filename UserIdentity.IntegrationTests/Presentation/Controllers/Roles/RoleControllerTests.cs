using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using UserIdentity.IntegrationTests.Persistence;
using UserIdentity.IntegrationTests.Presentation.Helpers;
using UserIdentity.IntegrationTests.TestUtils;

using Xunit;
using Xunit.Abstractions;

namespace UserIdentity.IntegrationTests.Presentation.Controllers.Roles
{
	public class RoleControllerTests: BaseControllerTests
	{
		private readonly static string _baseUri = "/api/v1/role";

		public RoleControllerTests(TestingWebAppFactory testingWebAppFactory, ITestOutputHelper outputHelper)
				: base(testingWebAppFactory, outputHelper)
		{
		}

		[Fact]
		public async Task Get_Roles_Returns_Roles()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);
			
			var additionalRoleId =  Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			_outputHelper.WriteLine(jsonObject.ToString());

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);
		}
	}
}
