using PolyzenKit.Common.Utilities;

namespace UserIdentity.UnitTests.TestUtils
{
	internal static class TestStringHelper
	{
		internal static string UserId = StringUtil.GenerateRandomString(32);
		internal static string UserName = StringUtil.GenerateRandomString(40);
	}
}
