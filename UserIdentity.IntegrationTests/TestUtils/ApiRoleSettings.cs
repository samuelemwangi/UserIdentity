using System;
using System.Linq;

namespace UserIdentity.IntegrationTests.TestUtils;

internal class ApiRoleSettings
{
	public static string ServiceName = "useridentity";
	public static string RolePrefix = $"{ServiceName}:";

	public static string RoleId = Guid.NewGuid().ToString();
	public static string RoleNameBase = "default";
	public static string RoleName => $"{RolePrefix}{RoleNameBase}";

	public static string DefaultRoleBase = "default";
	public static string DefaultRole => $"{RolePrefix}{DefaultRoleBase}";

	public static string AdminRolesBase = "administrator,super-administrator";
	public static string AdminRoles => string.Join(',', AdminRolesBase.Split(',').Select(r => $"{RolePrefix}{r.Trim()}"));
}
