using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json.Linq;

using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.IntegrationTests.Persistence;
using UserIdentity.IntegrationTests.Presentation.Helpers;
using UserIdentity.IntegrationTests.TestUtils;
using UserIdentity.Presentation.Helpers.ValidationExceptions;

using Xunit;

namespace UserIdentity.IntegrationTests.Presentation.Controllers
{
	public class UserControllerTests : IClassFixture<TestingWebAppFactory>, IDisposable
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

		[Fact]
		public async Task Refresh_Valid_User_Token_Refreshes_User_token()
		{
			// Arrange
			var appDbContext = ServiceResolver.ResolveDBContext(_serviceProvider);
			DBContexUtils.SeedDatabase(appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				AccessToken = userToken,
				RefreshToken = refreshToken
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/refresh-token");
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
			Assert.Equal("Refresh token generated successfully", jsonObject["statusMessage"]);


			var userTokenVM = jsonObject["userToken"]?.ToObject<AccessTokenViewModel>();

			Assert.NotNull(userTokenVM);
			Assert.IsType<AccessTokenDTO>(userTokenVM.AccessToken);

			Assert.NotEqual(userToken, userTokenVM.AccessToken.Token);
			Assert.NotEqual(refreshToken, userTokenVM.RefreshToken);

			Assert.True(userTokenVM?.AccessToken.ExpiresIn > 0);
		}

		[Fact]
		public async Task Refresh_User_Token_With_Invalid_Refresh_Token_Does_Not_Refresh_User_token()
		{
			// Arrange
			var appDbContext = ServiceResolver.ResolveDBContext(_serviceProvider);
			DBContexUtils.SeedDatabase(appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				AccessToken = userToken,
				RefreshToken = Base64UrlEncoder.Encode(refreshToken)
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/refresh-token");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();
			DBContexUtils.ClearDatabase(appDbContext);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("401 - UNAUTHORIZED", jsonObject["statusMessage"]);

			Assert.Equal("Invalid refresh token provided", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		[Fact]
		public async Task Refresh_User_Token_With_Invalid_Access_Token_Does_Not_Refresh_User_token()
		{
			// Arrange
			var appDbContext = ServiceResolver.ResolveDBContext(_serviceProvider);
			DBContexUtils.SeedDatabase(appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.Username, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				AccessToken = "eyJhbGciOiJIUzI1NiIsImtpZCI6IlFWQlFYMHRGV1Y5SlJBIiwidHlwIjoiSldUIiwiY3R5IjoiSldUIn0.eyJzdWIiOiJ0ZXN0LnVzZXIiLCJqdGkiOiJjNmRhNjY4Yy0xMzEzLTRhOTItOTQzYy1hMzVlMjA3MTYwZWEiLCJpYXQiOjE2Nzg3MDg0NzcsImlkIjoiNWZhODk4MjAtZTViMS00NmY2LWEwZmItYmQwMTk2NGY2NzRmIiwicm9sZXMiOiJBZG1pbmlzdHJhdG9yIiwibmJmIjoxNjc4NzA4NDc3LCJleHAiOjE2Nzg3MDg0OTIsImlzcyI6IklOVkFMSURfSVNTVUVSIiwiYXVkIjoiQVBQX0FVRElFTkNFIn0.Bcyy1ty8Uqa6duR8NrVolF4DlCfkKptWYP_oVwDVC5k",
				RefreshToken = refreshToken
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/refresh-token");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();
			DBContexUtils.ClearDatabase(appDbContext);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("401 - UNAUTHORIZED", jsonObject["statusMessage"]);

			Assert.Equal("An invalid access token was provided", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		[Fact]
		public async Task Reset_Password_With_Invalid_Payload_Returns_Validation_Results()
		{
			// Arrange
			var requestPayload = new
			{
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/reset-password");
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

			Assert.Equal(1, errorList?.Count);

			Assert.True(errorList?.Any(x => x.Field == "UserEmail" && x.Message == "The UserEmail field is required.") ?? false);
		}

		[Fact]
		public async Task Reset_Password_For_Existing_User_Resets_Password()
		{
			// Arrange
			var requestPayload = new
			{
				UserSettings.UserEmail,
			};

			var appDbContext = ServiceResolver.ResolveDBContext(_serviceProvider);
			DBContexUtils.SeedDatabase(appDbContext);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/reset-password");
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


			var resetPasswordDTO = jsonObject["resetPasswordDetails"]?.ToObject<ResetPasswordDTO>();

			Assert.NotNull(resetPasswordDTO);
			Assert.Equal("APP_DEFAULT_RESET_PASSWORD_MESSAGE", resetPasswordDTO?.EmailMessage);
		}

		[Fact]
		public async Task Reset_Password_For_Non_Existing_User_Does_Not_Reset_Password()
		{
			// Arrange
			var requestPayload = new
			{
				UserEmail = "hello@test.com",
			};

			var appDbContext = ServiceResolver.ResolveDBContext(_serviceProvider);
			DBContexUtils.SeedDatabase(appDbContext);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/reset-password");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

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

			Assert.Equal($"No record exists for the provided identifier - {requestPayload.UserEmail}", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		[Fact]
		public async Task Reset_Password_For_Non_Existing_UserDetails_Record_Does_Not_Reset_Password()
		{
			// Arrange
			var requestPayload = new
			{
				UserSettings.UserEmail
			};

			var appDbContext = ServiceResolver.ResolveDBContext(_serviceProvider);
			DBContexUtils.SeedIdentityUser(appDbContext);
			

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/reset-password");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();
			DBContexUtils.ClearDatabase(appDbContext);

			// Assert
			Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);


			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("500 - INTERNAL SERVER ERROR", jsonObject["statusMessage"]);

			Assert.Equal($"An error occured while updating a record identified by - {requestPayload.UserEmail}", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		public void Dispose()
		{
			var appDbContext = ServiceResolver.ResolveDBContext(_serviceProvider);
			appDbContext.Dispose();
		}
	}
}
