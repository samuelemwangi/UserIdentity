using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using UserIdentity.Application.Core.Errors.ViewModels;
using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.IntegrationTests.Persistence;
using UserIdentity.IntegrationTests.Presentation.Helpers;
using UserIdentity.IntegrationTests.TestUtils;
using UserIdentity.Presentation.Helpers.ValidationExceptions;

using Xunit;

namespace UserIdentity.IntegrationTests.Presentation.Controllers
{
	public class UserControllerTests : IClassFixture<TestingWebAppFactory>
	{

		private readonly HttpClient _httpClient;
		private readonly IServiceProvider _serviceProvider;
		private readonly static String _baseUri = "/api/v1/user";

		public UserControllerTests(TestingWebAppFactory testingWebAppFactory)
		{
			_httpClient = testingWebAppFactory.CreateClient();
			_serviceProvider = testingWebAppFactory.Services;
		}

		[Fact]
		public async Task Get_Existing_User_Gets_User_Details()
		{
			// Arrange
			var appDbContext = ServiceResolver.ResolveDBContext(_serviceProvider);
			DBContexUtils.SeedDatabase(appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/" + UserSettings.UserId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();
			DBContexUtils.ClearDatabase(appDbContext);

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);


			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			var userDetails = jsonObject["user"]?.ToObject<UserDTO>();

			Assert.Equal(UserSettings.FirstName + " " + UserSettings.LastName, userDetails?.FullName);
			Assert.Equal(UserSettings.Username, userDetails?.UserName);
			Assert.Equal(UserSettings.UserEmail, userDetails?.Email);
			Assert.Equal(userDetails?.Id, userDetails?.CreatedBy);
			Assert.Equal(userDetails?.Id, userDetails?.LastModifiedBy);
		}

		[Fact]
		public async Task Get_Non_Existing_User_Does_Not_Get_User_Details()
		{
			// Arrange
			var appDbContext = ServiceResolver.ResolveDBContext(_serviceProvider);
			DBContexUtils.SeedDatabase(appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var nonExistingUserId = Guid.NewGuid().ToString();

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/" + nonExistingUserId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();
			DBContexUtils.ClearDatabase(appDbContext);

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);


			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("404 - NOT FOUND", jsonObject["statusMessage"]);

			Assert.Equal($"No record exists for the provided identifier - {nonExistingUserId}", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		[Fact]
		public async Task Register_User_With_Invalid_Request_Payload_Returns_Validation_Errors()
		{
			// Arrange
			var requestPayload = new
			{
				UserSettings.LastName,
				UserSettings.PhoneNumber,
				UserSettings.UserEmail,
				UserSettings.UserPassword,
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/register");

			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

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

			Assert.True(errorList?.Any(x => x.Field == "FirstName" && x.Message == "The FirstName field is required.") ?? false);
			Assert.True(errorList?.Any(x => x.Field == "Username" && x.Message == "The Username field is required.") ?? false);
		}

		[Fact]
		public async Task Register_User_With_Valid_Request_Payload_Creates_User()
		{
			// Arrange
			var requestPayload = new
			{
				UserSettings.FirstName,
				UserSettings.LastName,
				UserSettings.Username,
				UserSettings.PhoneNumber,
				UserSettings.UserEmail,
				UserSettings.UserPassword

			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/register");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			var userDetails = jsonObject["userDetails"]?.ToObject<UserDTO>();

			Assert.Equal(requestPayload.FirstName + " " + requestPayload.LastName, userDetails?.FullName);
			Assert.Equal(requestPayload.Username, userDetails?.UserName);
			Assert.Equal(requestPayload.UserEmail, userDetails?.Email);
			Assert.Equal(userDetails?.Id, userDetails?.CreatedBy);
			Assert.Equal(userDetails?.Id, userDetails?.LastModifiedBy);

			var userToken = jsonObject["userToken"]?.ToObject<AccessTokenViewModel>();

			Assert.NotNull(userToken);
			Assert.IsType<AccessTokenDTO>(userToken.AccessToken);
		}

		[Fact]
		public async Task Register_Existing_User_With_Valid_Request_Payload_Does_Not_Create_User()
		{
			// Arrange
			var requestPayload = new
			{
				UserSettings.FirstName,
				UserSettings.LastName,
				UserSettings.Username,
				UserSettings.PhoneNumber,
				UserSettings.UserEmail,
				UserSettings.UserPassword

			};


			var appDbContext = ServiceResolver.ResolveDBContext(_serviceProvider);
			DBContexUtils.SeedDatabase(appDbContext);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/register");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			DBContexUtils.ClearDatabase(appDbContext);

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("400 - BAD REQUEST", jsonObject["statusMessage"]);

			Assert.Equal($"A record identified with - {requestPayload.Username} - exists", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		[Fact]
		public async Task Login_Existing_User_Logs_In_User()
		{
			// Arrange
			var requestPayload = new
			{
				UserName = UserSettings.Username,
				Password = UserSettings.UserPassword
			};

			var appDbContext = ServiceResolver.ResolveDBContext(_serviceProvider);
			DBContexUtils.SeedDatabase(appDbContext);


			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/login");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			DBContexUtils.ClearDatabase(appDbContext);

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item(s) fetched successfully", jsonObject["statusMessage"]);

			var userDetails = jsonObject["userDetails"]?.ToObject<UserDTO>();

			Assert.Equal(UserSettings.FirstName + " " + UserSettings.LastName, userDetails?.FullName);
			Assert.Equal(requestPayload.UserName, userDetails?.UserName);
			Assert.Equal(UserSettings.UserEmail, userDetails?.Email);
			Assert.Equal(userDetails?.Id, userDetails?.CreatedBy);
			Assert.Equal(userDetails?.Id, userDetails?.LastModifiedBy);

			var userToken = jsonObject["userToken"]?.ToObject<AccessTokenViewModel>();

			Assert.NotNull(userToken);
			Assert.IsType<AccessTokenDTO>(userToken.AccessToken);
		}

		[Fact]
		public async Task Login_Non_Existing_User_Does_Not_Log_In_User()
		{
			// Arrange
			var requestPayload = new
			{
				UserName = UserSettings.Username,
				Password = UserSettings.UserPassword
			};


			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/login");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("401 - UNAUTHORIZED", jsonObject["statusMessage"]);

			Assert.Equal("Provided username and password combination is invalid", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}
	}
}
