using System;

namespace UserIdentity.IntegrationTests.TestUtils
{
	internal class RoleSettings
	{
		public static string RoleId = Guid.NewGuid().ToString();
		public static string RoleName = "role:useridentity:administrator";
	}
}
