using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using UserIdentity.Application.Core.Tokens.ViewModels;

namespace UserIdentity.IntegrationTests.Presentation.Helpers
{
	internal static class APIHelper
	{
		public static String loginUrl = "/api/v1/user/login";

		public static async Task<(String?, String?)> LoginUserAsync(this HttpClient httpClient, String username, String userPassword)
		{
			// Arrange
			var requestPayload = new
			{
				UserName = username,
				Password = userPassword
			};

			var httpRequest = CreateHttpRequestMessage(HttpMethod.Post, loginUrl);
			httpRequest.Content = SerDe.ConvertToHttpContent(requestPayload);

			var response = await httpClient.SendAsync(httpRequest);
			var responseString = await response.Content.ReadAsStringAsync();

			var jsonObject = SerDe.Deserialize<JObject>(responseString);

			if (jsonObject == null)
				return (null as String, null as String);

			var userToken = jsonObject["userToken"]?.ToObject<AccessTokenViewModel>();

			return (userToken?.AccessToken?.Token, userToken?.RefreshToken);
		}

		public static HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, String uri)
		{
			return new HttpRequestMessage(new HttpMethod(httpMethod.Method), uri);
		}
		public static void AddAuthHeader(this HttpRequestMessage httpRequest, String authToken)
		{
			httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
		}

	}
}
