﻿using Microsoft.EntityFrameworkCore;

using UserIdentity.Persistence.Infrastructure;

namespace UserIdentity.Persistence.Infrastructure
{
	public static class ContextExtensions
	{
		public static String? GetTableName<T>(this DbContext context) where T : class
		{
			var entityType = context.Model.FindEntityType(typeof(T));
			return entityType?.GetTableName();
		}

		public static String? GetSchemaName<T>(this DbContext context) where T : class
		{
			var entityType = context.Model.FindEntityType(typeof(T));
			return entityType?.GetSchema();
		}
	}
}
