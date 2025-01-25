using System;

namespace UserIdentity.IntegrationTests.TestUtils
{
	internal class ApiScopeClaimSettings
	{
		public static string Resource = "user";
		public static string Action = "edit";
		public static string ScopeClaim = $"{Resource}:{Action}";
	}
}
