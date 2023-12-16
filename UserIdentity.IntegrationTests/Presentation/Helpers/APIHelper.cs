using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using UserIdentity.Application.Core.Tokens.ViewModels;

using Xunit;

namespace UserIdentity.IntegrationTests.Presentation.Helpers
{
	internal static class APIHelper
	{
		public static string loginUrl = "/api/v1/user/login";

		public static async Task<(string?, string?)> LoginUserAsync(this HttpClient httpClient, string userName, string userPassword)
		{
			// Arrange
			var requestPayload = new
			{
				UserName = userName,
				Password = userPassword
			};

			var httpRequest = CreateHttpRequestMessage(HttpMethod.Post, loginUrl);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			var response = await httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			if (jsonObject == null)
				return (null as string, null as string);

			var userToken = jsonObject["userToken"]?.ToObject<AccessTokenViewModel>();

			return (userToken?.AccessToken?.Token, userToken?.RefreshToken);
		}

		public static HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, string uri)
		{
			return new HttpRequestMessage(new HttpMethod(httpMethod.Method), uri);
		}
		public static void AddAuthHeader(this HttpRequestMessage httpRequest, string authToken)
		{
			httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
		}

		public static void ValidateRequestResponse(this HttpResponseMessage httpResponse)
		{
			var requestIdHeader = httpResponse.Headers.GetValues("X-Request-Id");
			Assert.NotNull(requestIdHeader);
			Assert.NotEmpty(requestIdHeader);
			Assert.Single(requestIdHeader);
		}

	}
}
