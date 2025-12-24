using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace UserIdentity.UnitTests.Persistence.Configurations;

internal static class EntityConfigurationPropertiesTestsHelper
{

  public static bool ConfirmMaxColumnLength(this EntityType entityType, string column, int columnLength)
  {
    var properies = entityType.GetProperties();

    foreach (var prop in properies)
    {
      if (prop.Name == column)
      {
        return prop.GetMaxLength() == columnLength;
      }
    }

    return false;
  }

  public static bool ConfirmColumnHasIndex(this EntityType entityType, string column)
  {
    var properies = entityType.GetProperties();

    foreach (var prop in properies)
    {
      if (prop.Name == column)
      {
        return prop.IsIndex();
      }
    }

    return false;
  }

  public static bool ConfirmColumnHasKey(this EntityType entityType, string column)
  {
    var properies = entityType.GetProperties();

    foreach (var prop in properies)
    {
      if (prop.Name == column)
      {
        return prop.IsKey();
      }
    }

    return false;
  }


}
