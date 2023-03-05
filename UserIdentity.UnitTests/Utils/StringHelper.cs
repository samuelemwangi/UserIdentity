using System;
using System.Text;

namespace UserIdentity.UnitTests.Utils
{
	internal static class StringHelper
	{
		public static string GenerateRandomString(Int32 length)
		{
			const String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var random = new Random();
			var result = new StringBuilder(length);

			for (Int32 i = 0; i < length; i++)
			{
				result.Append(chars[random.Next(chars.Length)]);
			}

			return result.ToString();

		}

	}
}
