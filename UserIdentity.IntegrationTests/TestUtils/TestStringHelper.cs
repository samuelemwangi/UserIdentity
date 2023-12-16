using System;
using System.Text;

namespace UserIdentity.IntegrationTests.TestUtils
{
	internal static class TestStringHelper
	{
		public static string GenerateRandomString(int length, bool useSpecialChars = true)
		{
			string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

			if (useSpecialChars)
				chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!£$%^&*()+-@#@><?";

			var random = new Random();
			var result = new StringBuilder(length);

			for (int i = 0; i < length; i++)
			{
				result.Append(chars[random.Next(chars.Length)]);
			}

			return result.ToString();

		}

	}
}
