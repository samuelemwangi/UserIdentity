using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.IntegrationTests.Persistence;
using UserIdentity.IntegrationTests.Presentation.Helpers;
using UserIdentity.IntegrationTests.TestUtils;
using UserIdentity.Presentation.Helpers.ValidationExceptions;

using Xunit;
using Xunit.Abstractions;

namespace UserIdentity.IntegrationTests.Presentation.Controllers.Users
{

	public class UserControllerTests : BaseControllerTests
	{

		private readonly static String _baseUri = "/api/v1/user";

		public UserControllerTests(TestingWebAppFactory testingWebAppFactory, ITestOutputHelper outputHelper)
				: base(testingWebAppFactory, outputHelper)
		{
		}

		[Fact]
		public async Task Get_Existing_User_Gets_User_Details()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.UserName, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/" + UserSettings.UserId);
			httpRequest.AddAuthHeader(userToken);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);


			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item fetched successfully", jsonObject["statusMessage"]);

			var userDetails = jsonObject["user"]?.ToObject<UserDTO>();

			Assert.Equal(UserSettings.FirstName + " " + UserSettings.LastName, userDetails?.FullName);
			Assert.Equal(UserSettings.UserName, userDetails?.UserName);
			Assert.Equal(UserSettings.UserEmail, userDetails?.Email);
			Assert.Equal(userDetails?.Id, userDetails?.CreatedBy);
			Assert.Equal(userDetails?.Id, userDetails?.UpdatedBy);
		}

		[Fact]
		public async Task Get_Non_Existing_User_Does_Not_Get_User_Details()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.UserName, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var nonExistingUserId = Guid.NewGuid().ToString();

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/" + nonExistingUserId);
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

			Assert.Equal($"No record exists for the provided identifier - {nonExistingUserId}", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		[Fact]
		public async Task Get_Non_Existing_AppUser_Does_Not_Get_User_Details()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.UserName, UserSettings.UserPassword);

			DBContexUtils.ClearAppUser(_appDbContext);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var nonExistingUserId = Guid.NewGuid().ToString();

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/" + nonExistingUserId);
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

			Assert.Equal($"No record exists for the provided identifier - {nonExistingUserId}", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		[Fact]
		public async Task Get_Deleted_AppUser_Does_Not_Get_User_Details()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.UserName, UserSettings.UserPassword);

			DBContexUtils.ClearAppUser(_appDbContext);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var nonExistingUserId = Guid.NewGuid().ToString();

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Get, _baseUri + "/" + nonExistingUserId);
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
				UserSettings.UserEmail
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

			Assert.Equal(3, errorList?.Count);

			Assert.True(errorList?.Any(x => x.Field == "FirstName" && x.Message == "The FirstName field is required.") ?? false);
			Assert.True(errorList?.Any(x => x.Field == "UserName" && x.Message == "The UserName field is required.") ?? false);
			Assert.True(errorList?.Any(x => x.Field == "UserPassword" && x.Message == "The UserPassword field is required.") ?? false);
		}

		[Fact]
		public async Task Register_User_With_Invalid_Request_Missing_Email_Phone_Number_Payload_Returns_Validation_Errors()
		{
			// Arrange
			var requestPayload = new
			{
				UserSettings.FirstName,
				UserSettings.UserName,
				UserSettings.UserPassword
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

			Assert.True(errorList?.Any(x => x.Field == "UserEmail" && x.Message == "Either PhoneNumber or UserEmail must be provided.") ?? false);
			Assert.True(errorList?.Any(x => x.Field == "PhoneNumber" && x.Message == "Either PhoneNumber or UserEmail must be provided.") ?? false);
		}

		[Fact]
		public async Task Register_User_With_Valid_Request_Payload_Registers_User()
		{
			// Arrange
			var requestPayload = new
			{
				UserSettings.FirstName,
				UserSettings.LastName,
				UserSettings.UserName,
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
			Assert.Equal("Item created successfully", jsonObject["statusMessage"]);

			var userDetails = jsonObject["userDetails"]?.ToObject<UserDTO>();

			Assert.Equal(requestPayload.FirstName + " " + requestPayload.LastName, userDetails?.FullName);
			Assert.Equal(requestPayload.UserName, userDetails?.UserName);
			Assert.Equal(requestPayload.UserEmail, userDetails?.Email);
			Assert.Equal(userDetails?.Id, userDetails?.CreatedBy);
			Assert.Equal(userDetails?.Id, userDetails?.UpdatedBy);

			var userToken = jsonObject["userToken"]?.ToObject<AccessTokenViewModel>();

			Assert.NotNull(userToken);
			Assert.IsType<AccessTokenDTO>(userToken.AccessToken);
			Assert.Equal(_props["APP_VALID_FOR"], userToken.AccessToken.ExpiresIn + "");
		}

		[Theory]
		[InlineData("random@test.com", null)]
		[InlineData("random@test.com", "")]
		[InlineData(null, "712121212")]
		[InlineData("", "712121212")]
		public async Task Register_User_With_Only_Required_Request_Payload_Registers_User(String UserEmail, String PhoneNumber)
		{
			// Arrange
			var requestPayload = new
			{
				UserSettings.FirstName,
				UserSettings.UserName,
				UserSettings.UserPassword,
				UserEmail,
				PhoneNumber
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/register");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();
			_outputHelper.WriteLine(responseString);

			// Assert
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);


			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Item created successfully", jsonObject["statusMessage"]);

			var userDetails = jsonObject["userDetails"]?.ToObject<UserDTO>();

			_outputHelper.WriteLine(responseString);

			Assert.Equal(requestPayload.FirstName, userDetails?.FullName);
			Assert.Equal(requestPayload.UserName, userDetails?.UserName);
			Assert.Equal(UserEmail, userDetails?.Email);
			Assert.Equal(userDetails?.Id, userDetails?.CreatedBy);
			Assert.Equal(userDetails?.Id, userDetails?.UpdatedBy);

			var userToken = jsonObject["userToken"]?.ToObject<AccessTokenViewModel>();

			Assert.NotNull(userToken);
			Assert.IsType<AccessTokenDTO>(userToken.AccessToken);
			Assert.Equal(_props["APP_VALID_FOR"], userToken.AccessToken.ExpiresIn + "");
		}

		[Fact]
		public async Task Register_Existing_User_With_Valid_Request_Payload_Does_Not_Create_User()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);


			var requestPayload = new
			{
				UserSettings.FirstName,
				UserSettings.LastName,
				UserSettings.UserName,
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
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("400 - BAD REQUEST", jsonObject["statusMessage"]);

			Assert.Equal($"A record identified with - {requestPayload.UserName} - exists", jsonObject["error"]?["message"]);

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
			DBContexUtils.SeedDatabase(_appDbContext);

			var requestPayload = new
			{
				UserName = UserSettings.UserName,
				Password = UserSettings.UserPassword
			};


			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/login");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Login successful", jsonObject["statusMessage"]);

			var userDetails = jsonObject["userDetails"]?.ToObject<UserDTO>();

			Assert.Equal(UserSettings.FirstName + " " + UserSettings.LastName, userDetails?.FullName);
			Assert.Equal(requestPayload.UserName, userDetails?.UserName);
			Assert.Equal(UserSettings.UserEmail, userDetails?.Email);
			Assert.Equal(userDetails?.Id, userDetails?.CreatedBy);
			Assert.Equal(userDetails?.Id, userDetails?.UpdatedBy);

			var userToken = jsonObject["userToken"]?.ToObject<AccessTokenViewModel>();

			Assert.NotNull(userToken);
			Assert.IsType<AccessTokenDTO>(userToken.AccessToken);
			Assert.Equal(_props["APP_VALID_FOR"], userToken.AccessToken.ExpiresIn + "");
		}

		[Fact]
		public async Task Login_Non_Existing_User_Does_Not_Log_In_User()
		{
			// Arrange
			var requestPayload = new
			{
				UserName = UserSettings.UserName,
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

			Assert.Equal("Provided credentials are invalid", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		[Fact]
		public async Task Login_Existing_Non_Existent_AppUser_Does_Not_Log_In_User()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);
			DBContexUtils.ClearAppUser(_appDbContext);

			var requestPayload = new
			{
				UserName = UserSettings.UserName,
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

			Assert.Equal("Provided credentials are invalid", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		[Fact]
		public async Task Login_Existing_Deleted_AppUser_Does_Not_Log_In_User()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);
			DBContexUtils.DeleteAppUser(_appDbContext);

			var requestPayload = new
			{
				UserName = UserSettings.UserName,
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

			Assert.Equal("Provided credentials are invalid", jsonObject["error"]?["message"]);

			var dateTime = (DateTime?)jsonObject["error"]?["timestamp"];

			Assert.NotNull(dateTime);
			Assert.Equal(DateTime.UtcNow.Year, dateTime.Value.Year);
			Assert.Equal(DateTime.UtcNow.Month, dateTime.Value.Month);
			Assert.Equal(DateTime.UtcNow.Day, dateTime.Value.Day);
		}

		[Fact]
		public async Task Login_Existing_With_Invalid_Password_Does_Not_Log_In_User()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var requestPayload = new
			{
				UserName = UserSettings.UserName,
				Password = UserSettings.UserPassword + "123"
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

			Assert.Equal("Provided credentials are invalid", jsonObject["error"]?["message"]);

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
			DBContexUtils.SeedDatabase(_appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.UserName, UserSettings.UserPassword);

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
			DBContexUtils.SeedDatabase(_appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.UserName, UserSettings.UserPassword);

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
		public async Task Refresh_User_Token_With_Deleted_Refresh_Token_Does_Not_Refresh_User_token()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.UserName, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			DBContexUtils.DeleteRefreshToken(_appDbContext);

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
			DBContexUtils.SeedDatabase(_appDbContext);

			(var userToken, var refreshToken) = await _httpClient.LoginUserAsync(UserSettings.UserName, UserSettings.UserPassword);

			Assert.NotNull(userToken);
			Assert.NotNull(refreshToken);

			var requestPayload = new
			{
				AccessToken = UserSettings.InvalidUserToken,
				RefreshToken = refreshToken
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/refresh-token");
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
			DBContexUtils.SeedDatabase(_appDbContext);

			var requestPayload = new
			{
				UserSettings.UserEmail,
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/reset-password");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);


			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Password reset request successful", jsonObject["statusMessage"]);


			var resetPasswordDTO = jsonObject["resetPasswordDetails"]?.ToObject<ResetPasswordDTO>();

			Assert.NotNull(resetPasswordDTO);
			Assert.Equal(_props["APP_DEFAULT_RESET_PASSWORD_MESSAGE"], resetPasswordDTO?.EmailMessage);
		}

		[Fact]
		public async Task Reset_Password_For_Non_Existing_User_Does_Not_Reset_Password()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);

			var requestPayload = new
			{
				UserEmail = "hello@test.com",
			};


			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/reset-password");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

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
			DBContexUtils.SeedIdentityUser(_appDbContext);

			var requestPayload = new
			{
				UserSettings.UserEmail
			};


			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/reset-password");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

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


		[Fact]
		public async Task Confirm_Password_Token_With_Invalid_Payload_Returns_Validation_Results()
		{
			// Arrange
			var requestPayload = new
			{
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/confirm-update-password-token");
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

			Assert.True(errorList?.Any(x => x.Field == "ConfirmPasswordToken" && x.Message == "The ConfirmPasswordToken field is required.") ?? false);
			Assert.True(errorList?.Any(x => x.Field == "UserId" && x.Message == "The UserId field is required.") ?? false);
		}

		[Fact]
		public async Task Confirm_Password_Token_With_Valid_Payload_Confirms_Password_Token()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);
			var resetPasswordToken = DBContexUtils.UpdateResetPasswordToken(_appDbContext, _userManager);

			var requestPayload = new
			{
				ConfirmPasswordToken = resetPasswordToken,
				UserSettings.UserId
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/confirm-update-password-token");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Token confirmation successful", jsonObject["statusMessage"]);


			var resetPasswordDTO = jsonObject["tokenPasswordResult"]?.ToObject<ConfirmUpdatePasswordDTO>();

			Assert.NotNull(resetPasswordDTO);
			Assert.True(resetPasswordDTO?.UpdatePasswordTokenConfirmed);
		}


		[Fact]
		public async Task Confirm_Password_Token_With_Invalid_Token_Does_Not_Confirm_Password_Token()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);
			var resetPasswordToken = DBContexUtils.UpdateResetPasswordToken(_appDbContext, _userManager);

			var requestPayload = new
			{
				ConfirmPasswordToken = resetPasswordToken + "897455\\f",
				UserSettings.UserId
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/confirm-update-password-token");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.NotAcceptable, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("Token confirmation failed", jsonObject["statusMessage"]);


			var resetPasswordDTO = jsonObject["tokenPasswordResult"]?.ToObject<ConfirmUpdatePasswordDTO>();

			Assert.NotNull(resetPasswordDTO);
			Assert.False(resetPasswordDTO?.UpdatePasswordTokenConfirmed);
		}

		[Fact]
		public async Task Confirm_Password_Token_With_Non_Existent_Token_Does_Not_Confirm_Password_Token()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);
			var resetPasswordToken = DBContexUtils.UpdateResetPasswordToken(_appDbContext, _userManager);

			var requestPayload = new
			{
				ConfirmPasswordToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetPasswordToken + "897455989")),
				UserSettings.UserId
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/confirm-update-password-token");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.NotAcceptable, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("Token confirmation failed", jsonObject["statusMessage"]);


			var resetPasswordDTO = jsonObject["tokenPasswordResult"]?.ToObject<ConfirmUpdatePasswordDTO>();

			Assert.NotNull(resetPasswordDTO);
			Assert.False(resetPasswordDTO?.UpdatePasswordTokenConfirmed);
		}

		[Fact]
		public async Task Update_Password_With_Invalid_Payload_Returns_Validation_Results()
		{
			// Arrange
			var requestPayload = new
			{
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/update-password");
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

			Assert.Equal(3, errorList?.Count);

			Assert.True(errorList?.Any(x => x.Field == "NewPassword" && x.Message == "The NewPassword field is required.") ?? false);
			Assert.True(errorList?.Any(x => x.Field == "UserId" && x.Message == "The UserId field is required.") ?? false);
			Assert.True(errorList?.Any(x => x.Field == "PasswordResetToken" && x.Message == "The PasswordResetToken field is required.") ?? false);
		}

		[Fact]
		public async Task Update_Password_For_Non_Existent_User_Does_Not_Update_Password()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);
			var resetPasswordToken = DBContexUtils.UpdateResetPasswordToken(_appDbContext, _userManager);

			var requestPayload = new
			{
				NewPassword = "12345",
				UserId = Guid.NewGuid().ToString(),
				PasswordResetToken = resetPasswordToken
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/update-password");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.NotAcceptable, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("Password update failed", jsonObject["statusMessage"]);


			var updatePasswordDTO = jsonObject["updatePasswordResult"]?.ToObject<UpdatePasswordDTO>();

			Assert.NotNull(updatePasswordDTO);
			Assert.False(updatePasswordDTO?.PassWordUpdated);
		}

		[Fact]
		public async Task Update_Password_With_Valid_Payload_Updates_Password()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);
			var resetPasswordToken = DBContexUtils.UpdateResetPasswordToken(_appDbContext, _userManager);


			var requestPayload = new
			{
				NewPassword = "12345",
				UserSettings.UserId,
				PasswordResetToken = resetPasswordToken
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/update-password");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Successful", jsonObject["requestStatus"]);
			Assert.Equal("Password updated successfully", jsonObject["statusMessage"]);


			var updatePasswordDTO = jsonObject["updatePasswordResult"]?.ToObject<UpdatePasswordDTO>();

			Assert.NotNull(updatePasswordDTO);
			Assert.True(updatePasswordDTO?.PassWordUpdated);
		}

		[Fact]
		public async Task Update_Password_With_Invalid_Token_Does_Not_Update_Password()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);
			var resetPasswordToken = DBContexUtils.UpdateResetPasswordToken(_appDbContext, _userManager);

			var requestPayload = new
			{
				NewPassword = "12345",
				UserSettings.UserId,
				PasswordResetToken = resetPasswordToken + "897455\\f",
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/update-password");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.NotAcceptable, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("Password update failed", jsonObject["statusMessage"]);


			var updatePasswordDTO = jsonObject["updatePasswordResult"]?.ToObject<UpdatePasswordDTO>();

			Assert.NotNull(updatePasswordDTO);
			Assert.False(updatePasswordDTO?.PassWordUpdated);
		}

		[Fact]
		public async Task Update_Password_With_Non_Existent_Token_Does_Not_Update_Password()
		{
			// Arrange
			DBContexUtils.SeedDatabase(_appDbContext);
			var resetPasswordToken = DBContexUtils.UpdateResetPasswordToken(_appDbContext, _userManager);

			var requestPayload = new
			{
				NewPassword = "12345",
				UserSettings.UserId,
				PasswordResetToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(TestStringHelper.GenerateRandomString(56))),
			};

			var httpRequest = APIHelper.CreateHttpRequestMessage(HttpMethod.Post, _baseUri + "/update-password");
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			// Act
			var response = await _httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.NotAcceptable, response.StatusCode);
			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			Assert.NotNull(jsonObject);

			Assert.Equal("Request Failed", jsonObject["requestStatus"]);
			Assert.Equal("Password update failed", jsonObject["statusMessage"]);


			var updatePasswordDTO = jsonObject["updatePasswordResult"]?.ToObject<UpdatePasswordDTO>();

			Assert.NotNull(updatePasswordDTO);
			Assert.False(updatePasswordDTO?.PassWordUpdated);
		}


	}
}
