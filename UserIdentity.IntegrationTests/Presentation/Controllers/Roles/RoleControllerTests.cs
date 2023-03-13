﻿using System;
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
		public async Task Create_Existing_Role_Returns_Does_Not_Create_Role()
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
	}
}
