using System;

namespace UserIdentity.IntegrationTests.TestUtils
{
	internal static class UserSettings
	{
		public static Guid UserId = Guid.NewGuid(); 
		public static String UserEmail = "test.user@domain.com";
		public static String Username = "test.user";
		public static String FirstName = "Test";
		public static String LastName = "User";
		public static String PhoneNumber = "0123456789";
		public static String UserPassword = "Password123";
	}
}
