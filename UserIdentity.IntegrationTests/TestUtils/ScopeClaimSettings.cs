using System;

namespace UserIdentity.IntegrationTests.TestUtils
{
	internal class ScopeClaimSettings
	{
		public static String Resource = "user";
		public static String Action = "edit";
		public static String ScopeClaim = $"{Resource}:{Action}";
	}
}
