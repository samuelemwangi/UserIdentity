using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.IntegrationTests.Persistence;
using UserIdentity.IntegrationTests.Presentation.Helpers;
using UserIdentity.IntegrationTests.TestUtils;
using UserIdentity.Presentation.Helpers.ValidationExceptions;

using Xunit;
using Xunit.Abstractions;

namespace UserIdentity.IntegrationTests.Presentation.Controllers.Roles
{

	public class RoleControllerTests : BaseControllerTests
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

			var additionalRoleId = Guid.NewGuid().ToString();
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


			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			var roles = jsonObject["roles"]?.ToObject<List<RoleDTO>>();

			Assert.NotNull(roles);
			Assert.Equal(2, roles.Count);

			Assert.Contains(roles, r => r.Id == RoleSettings.RoleId && r.Name == RoleSettings.RoleName);
			Assert.Contains(roles, r => r.Id == additionalRoleId && r.Name == additionalRolename);
		}

		[Fact]
		public async Task Get_Roles_With_No_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Get_Roles_With_Invalid_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri);
			httpRequest.AddAuthHeader(UserSettings.InvalidUserToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Get_Roles_With_Invalid_Permissions_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedIdentityUser(_appDbContext);
			var roleId = Guid.NewGuid().ToString();
			var roleName = "additionalRole";
			DBContexUtils.SeedIdentityRole(_appDbContext, roleId, roleName);
			DBContexUtils.SeedIdentityUserRole(_appDbContext, roleId);
			DBContexUtils.SeedAppUser(_appDbContext);
			DBContexUtils.SeedRefreshToken(_appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task Get_Role_Returns_Role_Details()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/" + additionalRoleId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			var role = jsonObject["role"]?.ToObject<RoleDTO>();

			Assert.NotNull(role);
			Assert.Equal(additionalRoleId, role.Id);
			Assert.Equal(additionalRolename, role.Name);
		}

		[Fact]
		public async Task Get_Role_With_No_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/" + additionalRoleId);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Get_Role_With_Invalid_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/" + additionalRoleId);
			httpRequest.AddAuthHeader(UserSettings.InvalidUserToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Get_Role_With_Invalid_Permissions_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedIdentityUser(_appDbContext);
			var roleId = Guid.NewGuid().ToString();
			var roleName = "additionalRole";
			DBContexUtils.SeedIdentityRole(_appDbContext, roleId, roleName);
			DBContexUtils.SeedIdentityUserRole(_appDbContext, roleId);
			DBContexUtils.SeedAppUser(_appDbContext);
			DBContexUtils.SeedRefreshToken(_appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/" + roleId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task Create_Valid_Role_Returns_Role_Details()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				RoleName = "newRole",
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			var role = jsonObject["role"]?.ToObject<RoleDTO>();

			Assert.NotNull(role);
			Assert.NotNull(role.Id);
			Assert.Equal(requestPayload.RoleName, role.Name);
		}

		[Fact]
		public async Task Create_Existing_Role_Does_Not_Create_Role()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				RoleName = additionalRolename,
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("400 - BAD REQUEST", jsonObject["statusMessage"]);

			Assert.Equal($"A record identified with - {requestPayload.RoleName} - exists", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}


		[Fact]
		public async Task Create_Role_With_Invalid_Payload_Returns_Validation_Result()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("400 - BAD REQUEST", jsonObject["statusMessage"]);

			Assert.Equal("Validation Failed", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);

			var errorList = jsonObject["error"]?["errorList"]?.ToObject<List<ValidationError>>();

			Assert.Equal(1, errorList?.Count);

			Assert.True(errorList?.Any(x => x.Field == "RoleName" && x.Message == "The RoleName field is required.") ?? false);
		}

		[Fact]
		public async Task Create_Role_With_No_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var requestPayload = new
			{
				RoleName = "newRole",
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Create_Role_With_Invalid_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var requestPayload = new
			{
				RoleName = "newRole",
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(UserSettings.InvalidUserToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Create_Role_With_Invalid_Permissions_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedIdentityUser(_appDbContext);
			var roleId = Guid.NewGuid().ToString();
			var roleName = "additionalRole";
			DBContexUtils.SeedIdentityRole(_appDbContext, roleId, roleName);
			DBContexUtils.SeedIdentityUserRole(_appDbContext, roleId);
			DBContexUtils.SeedAppUser(_appDbContext);
			DBContexUtils.SeedRefreshToken(_appDbContext);

			var requestPayload = new
			{
				RoleName = "newRole",
			};

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task Update_Existing_Role_Updates_Role()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				RoleId = additionalRoleId,
				RoleName = "UpdatednewRole",
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Put, _baseUri + "/" + requestPayload.RoleId);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			var role = jsonObject["role"]?.ToObject<RoleDTO>();

			Assert.NotNull(role);
			Assert.Equal(requestPayload.RoleId, role.Id);
			Assert.Equal(requestPayload.RoleName, role.Name);
		}

		[Fact]
		public async Task Update_Non_Existing_Role_Does_Not_Update_Role()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				RoleId = Guid.NewGuid().ToString(),
				RoleName = additionalRolename,
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Put, _baseUri + "/" + requestPayload.RoleId);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("404 - NOT FOUND", jsonObject["statusMessage"]);

			Assert.Equal($"No record exists for the provided identifier - {requestPayload.RoleId}", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}


		[Fact]
		public async Task Update_Role_With_Invalid_Payload_Returns_Validation_Result()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Put, _baseUri + "/" + additionalRoleId);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("400 - BAD REQUEST", jsonObject["statusMessage"]);

			Assert.Equal("Validation Failed", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);

			var errorList = jsonObject["error"]?["errorList"]?.ToObject<List<ValidationError>>();

			Assert.Equal(2, errorList?.Count);

			Assert.True(errorList?.Any(x => x.Field == "roleId" && x.Message == "The RoleId field is required.") ?? false);
			Assert.True(errorList?.Any(x => x.Field == "RoleName" && x.Message == "The RoleName field is required.") ?? false);
		}

		[Fact]
		public async Task Update_Role_With_No_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var requestPayload = new
			{
				RoleName = "UpdatednewRole",
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Put, _baseUri + "/" + additionalRoleId);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Update_Role_With_Invalid_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var requestPayload = new
			{
				RoleName = "UpdatedRole",
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Put, _baseUri + "/" + additionalRoleId);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(UserSettings.InvalidUserToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Update_Role_With_Invalid_Permissions_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedIdentityUser(_appDbContext);
			var roleId = Guid.NewGuid().ToString();
			var roleName = "additionalRole";
			DBContexUtils.SeedIdentityRole(_appDbContext, roleId, roleName);
			DBContexUtils.SeedIdentityUserRole(_appDbContext, roleId);
			DBContexUtils.SeedAppUser(_appDbContext);
			DBContexUtils.SeedRefreshToken(_appDbContext);

			var requestPayload = new
			{
				RoleName = "UpdatedRole",
			};

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Put, _baseUri + "/" + roleId);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task Delete_Role_Returns_Role_Details()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Delete, _baseUri + "/" + additionalRoleId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Record deleted successfully", jsonObject["statusMessage"]);

		}

		[Fact]
		public async Task Delete_Non_Existent_Role_Does_Not_Delete_Role()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var nonExistentRuleId = Guid.NewGuid().ToString();

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Delete, _baseUri + "/" + nonExistentRuleId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("404 - NOT FOUND", jsonObject["statusMessage"]);

			Assert.Equal($"No record exists for the provided identifier - {nonExistentRuleId}", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		[Fact]
		public async Task Delete_Role_With_No_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Delete, _baseUri + "/" + additionalRoleId);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Delete_Role_With_Invalid_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Delete, _baseUri + "/" + additionalRoleId);
			httpRequest.AddAuthHeader(UserSettings.InvalidUserToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Delete_Role_With_Invalid_Permissions_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedIdentityUser(_appDbContext);
			var roleId = Guid.NewGuid().ToString();
			var roleName = "additionalRole";
			DBContexUtils.SeedIdentityRole(_appDbContext, roleId, roleName);
			DBContexUtils.SeedIdentityUserRole(_appDbContext, roleId);
			DBContexUtils.SeedAppUser(_appDbContext);
			DBContexUtils.SeedRefreshToken(_appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Delete, _baseUri + "/" + roleId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task Get_User_Roles_Returns_User_Roles()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/user/" + UserSettings.UserId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);


			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			var roles = jsonObject["userRoles"]?.ToObject<List<String>>();

			Assert.NotNull(roles);
			Assert.Equal(1, roles?.Count);

			Assert.Contains(RoleSettings.RoleName, roles?[0]);
		}

		[Fact]
		public async Task Get_User_Non_Existent_User_Id_Does_Not_Delete_Role()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var nonExistentUserId = Guid.NewGuid().ToString();

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/user/" + nonExistentUserId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("404 - NOT FOUND", jsonObject["statusMessage"]);

			Assert.Equal($"No record exists for the provided identifier - {nonExistentUserId}", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		[Fact]
		public async Task Get_User_Roles_With_No_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/user/" + UserSettings.UserId);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Get_User_Roles_With_Invalid_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/user/" + UserSettings.UserId);
			httpRequest.AddAuthHeader(UserSettings.InvalidUserToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Get_User_Roles_With_Invalid_Permissions_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedIdentityUser(_appDbContext);
			var roleId = Guid.NewGuid().ToString();
			var roleName = "additionalRole";
			DBContexUtils.SeedIdentityRole(_appDbContext, roleId, roleName);
			DBContexUtils.SeedIdentityUserRole(_appDbContext, roleId);
			DBContexUtils.SeedAppUser(_appDbContext);
			DBContexUtils.SeedRefreshToken(_appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/user/" + UserSettings.UserId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task Create_Valid_User_Role_Returns_Role_Details()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				UserSettings.UserId,
				RoleId = additionalRoleId
			};


			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/user");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			var roles = jsonObject["userRoles"]?.ToObject<List<String>>();

			Assert.NotNull(roles);
			Assert.Equal(2, roles?.Count);

			Assert.True(roles?.Where(r => r == additionalRolename).Any());
			Assert.True(roles?.Where(r => r == RoleSettings.RoleName).Any());
		}

		[Fact]
		public async Task Create_Existing_User_Role_Does_Not_Create_Role()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				UserSettings.UserId,
				RoleSettings.RoleId
			};


			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/user");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("400 - BAD REQUEST", jsonObject["statusMessage"]);

			Assert.Equal($"A record identified with - {requestPayload.RoleId} - exists", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}


		[Fact]
		public async Task Create_User_Role_With_Invalid_Payload_Returns_Validation_Result()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/user");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("400 - BAD REQUEST", jsonObject["statusMessage"]);

			Assert.Equal("Validation Failed", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);

			var errorList = jsonObject["error"]?["errorList"]?.ToObject<List<ValidationError>>();

			Assert.Equal(2, errorList?.Count);

			Assert.True(errorList?.Any(x => x.Field == "UserId" && x.Message == "The UserId field is required.") ?? false);
			Assert.True(errorList?.Any(x => x.Field == "RoleId" && x.Message == "The RoleId field is required.") ?? false);
		}

		[Fact]
		public async Task Create_User_Role_With_No_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var requestPayload = new
			{
				UserSettings.UserId,
				RoleId = additionalRoleId
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/user");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Create_User_Role_With_Invalid_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);


			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var requestPayload = new
			{
				UserSettings.UserId,
				RoleId = additionalRoleId
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/user");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(UserSettings.InvalidUserToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Create_User_Role_With_Invalid_Permissions_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedIdentityUser(_appDbContext);
			var roleId = Guid.NewGuid().ToString();
			var roleName = "additionalRole";
			DBContexUtils.SeedIdentityRole(_appDbContext, roleId, roleName);
			DBContexUtils.SeedAppUser(_appDbContext);
			DBContexUtils.SeedRefreshToken(_appDbContext);

			var requestPayload = new
			{
				UserSettings.UserId,
				RoleId = roleId
			};


			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/user");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task Create_Valid_Role_Claim_Returns_Role_Details()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				RoleSettings.RoleId,
				Resource = "user",
				Action = "create",
			};


			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/claim");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			var roleClaim = jsonObject["roleClaim"]?.ToObject<RoleClaimDTO>();

			Assert.NotNull(roleClaim);
			Assert.Equal(requestPayload.Resource, roleClaim.Resource);
			Assert.Equal(requestPayload.Action, roleClaim.Action);
			Assert.Equal(requestPayload.Resource + ":" + requestPayload.Action, roleClaim.Scope);
		}

		[Fact]
		public async Task Create_Existing_User_Role_Claim_Does_Not_Create_Role_Claim()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var createRoleClaimResult = DBContexUtils.UpdateRoleClaim(_appDbContext, _roleManager);

			Assert.True(createRoleClaimResult);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				RoleSettings.RoleId,
				ScopeClaimSettings.Resource,
				ScopeClaimSettings.Action,
			};


			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/claim");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("400 - BAD REQUEST", jsonObject["statusMessage"]);

			Assert.Equal($"A record identified with - {ScopeClaimSettings.ScopeClaim} - exists", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}


		[Fact]
		public async Task Create_Role_Claim_With_Invalid_Payload_Returns_Validation_Result()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/claim");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("400 - BAD REQUEST", jsonObject["statusMessage"]);

			Assert.Equal("Validation Failed", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);

			var errorList = jsonObject["error"]?["errorList"]?.ToObject<List<ValidationError>>();

			Assert.Equal(3, errorList?.Count);

			Assert.True(errorList?.Any(x => x.Field == "RoleId" && x.Message == "The RoleId field is required.") ?? false);
			Assert.True(errorList?.Any(x => x.Field == "Resource" && x.Message == "The Resource field is required.") ?? false);
			Assert.True(errorList?.Any(x => x.Field == "Action" && x.Message == "The Action field is required.") ?? false);
		}

		[Fact]
		public async Task Create_Role_Claim_With_No_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var requestPayload = new
			{
				RoleSettings.RoleId,
				Resource = "user",
				Action = "create",
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/claim");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Create_Role_Claim_With_Invalid_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);


			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var requestPayload = new
			{
				RoleSettings.RoleId,
				Resource = "user",
				Action = "create",
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/claim");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(UserSettings.InvalidUserToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Create_Role_Claim_With_Invalid_Permissions_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedIdentityUser(_appDbContext);
			var roleId = Guid.NewGuid().ToString();
			var roleName = "additionalRole";
			DBContexUtils.SeedIdentityRole(_appDbContext, roleId, roleName);
			DBContexUtils.SeedAppUser(_appDbContext);
			DBContexUtils.SeedRefreshToken(_appDbContext);

			var requestPayload = new
			{
				RoleSettings.RoleId,
				Resource = "user",
				Action = "create",
			};


			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/claim");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}


		[Fact]
		public async Task Get_Role_Claims_Returns_Role_Claims()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);
			var createRoleClaimResult = DBContexUtils.UpdateRoleClaim(_appDbContext, _roleManager);

			Assert.True(createRoleClaimResult);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/claim/" + RoleSettings.RoleId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			var roleClaims = jsonObject["roleClaims"]?.ToObject<List<RoleClaimDTO>>();

			Assert.NotNull(roleClaims);
			Assert.Equal(1, roleClaims?.Count);
			Assert.Equal(ScopeClaimSettings.Resource, roleClaims?[0].Resource);
			Assert.Equal(ScopeClaimSettings.Action, roleClaims?[0].Action);
			Assert.Equal(ScopeClaimSettings.ScopeClaim, roleClaims?[0].Scope);
		}

		[Fact]
		public async Task Get_Non_Existent_Role_Claim_Does_Not_Return_Role_Claim()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var nonExistentRoleId = Guid.NewGuid().ToString();

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/claim/" + nonExistentRoleId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("404 - NOT FOUND", jsonObject["statusMessage"]);

			Assert.Equal($"No record exists for the provided identifier - {nonExistentRoleId}", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		[Fact]
		public async Task Get_Role_Claims_With_No_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/claim/" + RoleSettings.RoleId);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Get_Role_Claims_With_Invalid_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/claim/" + RoleSettings.RoleId);
			httpRequest.AddAuthHeader(UserSettings.InvalidUserToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Get_Role_Claims_With_Invalid_Permissions_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedIdentityUser(_appDbContext);
			var roleId = Guid.NewGuid().ToString();
			var roleName = "additionalRole";
			DBContexUtils.SeedIdentityRole(_appDbContext, roleId, roleName);
			DBContexUtils.SeedIdentityUserRole(_appDbContext, roleId);
			DBContexUtils.SeedAppUser(_appDbContext);
			DBContexUtils.SeedRefreshToken(_appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/claim/" + RoleSettings.RoleId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task Delete_Existing_User_Role_Claim_Deletes_Role_Claim()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var createRoleClaimResult = DBContexUtils.UpdateRoleClaim(_appDbContext, _roleManager);

			Assert.True(createRoleClaimResult);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				RoleSettings.RoleId,
				ScopeClaimSettings.Resource,
				ScopeClaimSettings.Action,
			};


			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Delete, _baseUri + "/claim");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Record deleted successfully", jsonObject["statusMessage"]);
		}


		[Fact]
		public async Task Delete_Non_Existing_Role_Claim_Does_Not_Delete_Role_Claim()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				RoleSettings.RoleId,
				Resource = "user",
				Action = "create",
			};


			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Delete, _baseUri + "/claim");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("404 - NOT FOUND", jsonObject["statusMessage"]);

			Assert.Equal($"No record exists for the provided identifier - {requestPayload.Resource}:{requestPayload.Action}", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}


		[Fact]
		public async Task Delete_Role_Claim_With_Invalid_Payload_Returns_Validation_Result()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Delete, _baseUri + "/claim");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("400 - BAD REQUEST", jsonObject["statusMessage"]);

			Assert.Equal("Validation Failed", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);

			var errorList = jsonObject["error"]?["errorList"]?.ToObject<List<ValidationError>>();

			Assert.Equal(3, errorList?.Count);

			Assert.True(errorList?.Any(x => x.Field == "RoleId" && x.Message == "The RoleId field is required.") ?? false);
			Assert.True(errorList?.Any(x => x.Field == "Resource" && x.Message == "The Resource field is required.") ?? false);
			Assert.True(errorList?.Any(x => x.Field == "Action" && x.Message == "The Action field is required.") ?? false);
		}

		[Fact]
		public async Task Delete_Role_Claim_With_No_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var requestPayload = new
			{
				RoleSettings.RoleId,
				Resource = "user",
				Action = "create",
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Delete, _baseUri + "/claim");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Delete_Role_Claim_With_Invalid_Auth_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);


			var additionalRoleId = Guid.NewGuid().ToString();
			var additionalRolename = "additionalRole";

			DBContexUtils.SeedIdentityRole(_appDbContext, additionalRoleId, additionalRolename);

			var requestPayload = new
			{
				RoleSettings.RoleId,
				Resource = "user",
				Action = "create",
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Delete, _baseUri + "/claim");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(UserSettings.InvalidUserToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task Delete_Role_Claim_With_Invalid_Permissions_Returns_Auth_Error()
		{
			// Arrange
			DBContexUtils.SeedIdentityUser(_appDbContext);
			var roleId = Guid.NewGuid().ToString();
			var roleName = "additionalRole";
			DBContexUtils.SeedIdentityRole(_appDbContext, roleId, roleName);
			DBContexUtils.SeedAppUser(_appDbContext);
			DBContexUtils.SeedRefreshToken(_appDbContext);

			var requestPayload = new
			{
				RoleSettings.RoleId,
				Resource = "user",
				Action = "create",
			};


			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Delete, _baseUri + "/claim");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);

			// Assert
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

	}
}
