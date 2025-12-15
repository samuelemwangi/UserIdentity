using PolyzenKit.Common.Utilities;

namespace UserIdentity.IntegrationTests.TestUtils;

internal class ApiScopeClaimSettings
{
    public static string Resource = "user";
    public static string Action = "edit";
    public static string ScopeClaim = $"{Resource}{ZenConstants.SCOPE_CLAIM_SEPARATOR}{Action}";
}
