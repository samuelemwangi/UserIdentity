using System;

namespace UserIdentity.IntegrationTests.TestUtils
{
	internal class RoleSettings
	{
		public static String RoleId = Guid.NewGuid().ToString();
		public static String RoleName = "role:useridentity:administrator";
	}
}
