using System;

namespace UserIdentity.UnitTests.TestUtils
{
	internal static class ObjectHelper
	{
		public static bool HasProperty(this object obj, string propertyName)
		{
			return obj.GetType().GetProperty(propertyName) != null;
		}
	}
}
