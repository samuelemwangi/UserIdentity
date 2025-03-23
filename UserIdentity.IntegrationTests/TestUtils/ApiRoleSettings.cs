using System;

namespace UserIdentity.IntegrationTests.TestUtils;

internal class ApiRoleSettings
{
	public static string RoleId = Guid.NewGuid().ToString();
	public static string RoleName = "role:useridentity:app-user";
	public static string DefaultRole = "role:useridentity:default";
	public static string AdminRoles = "role:useridentity:administrator,role:useridentity:super-administrator";
}
