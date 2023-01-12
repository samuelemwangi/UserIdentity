using Microsoft.EntityFrameworkCore;
using UserIdentity.Persistence.Infrastructure;

namespace UserIdentity.Persistence.Infrastructure
{
	public static class ContextExtensions
	{
		public static string? GetTableName<T>(this DbContext context) where T : class
		{
			try
			{
				var entityType = context.Model.FindEntityType(typeof(T));
				var schema = entityType?.GetSchema();

				return entityType?.GetTableName();
			}
			catch (Exception e)
			{

				return e.Message;
			}
		}
	}
}
