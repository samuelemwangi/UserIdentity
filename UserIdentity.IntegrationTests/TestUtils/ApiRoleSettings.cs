using System;
using System.Linq;

using PolyzenKit.Common.Utilities;

namespace UserIdentity.IntegrationTests.TestUtils;

internal static class ApiRoleSettings
{
    public static string ServiceName = "useridentity";
    public static string RolePrefix = $"{ServiceName}{ZenConstants.SERVICE_ROLE_SEPARATOR}";

    public static string RoleId = Guid.NewGuid().ToString();
    public static string RoleNameBase = "user";
    public static string RoleName => $"{RolePrefix}{RoleNameBase}";

    public static string AdminRolesBase = "admin";
    public static string AdminRoles => string.Join(',', AdminRolesBase.Split(',').Select(r => $"{RolePrefix}{r.Trim()}"));
}
