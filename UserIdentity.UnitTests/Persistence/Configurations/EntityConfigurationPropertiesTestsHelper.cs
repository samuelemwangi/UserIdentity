using System;

using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace UserIdentity.UnitTests.Persistence.Configurations
{
	internal static class EntityConfigurationPropertiesTestsHelper
	{

		public static Boolean ConfirmMaxColumnLength(this EntityType entityType, String column, Int32 columnLength)
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

		public static Boolean ConfirmColumnHasIndex(this EntityType entityType, String column)
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
	}
}
