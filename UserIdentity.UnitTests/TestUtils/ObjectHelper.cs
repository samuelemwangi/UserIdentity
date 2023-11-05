using System;

namespace UserIdentity.UnitTests.TestUtils
{
  internal static class ObjectHelper
  {
    public static bool HasProperty(this object obj, String propertyName)
    {
      return obj.GetType().GetProperty(propertyName) != null;
    }
  }
}
