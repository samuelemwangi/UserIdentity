using System;

using Microsoft.Extensions.DependencyInjection;

using UserIdentity.Persistence;

namespace UserIdentity.IntegrationTests
{
	internal static class ServiceResolver
	{
		//public static AppDbContext ResolveDBContext(IServiceProvider serviceProvider)
		//{
		//	var scope = serviceProvider.CreateScope();

		//	return scope.ServiceProvider.GetRequiredService<AppDbContext>();
		//}


		public static T ResolveService<T>(IServiceProvider serviceProvider) where T : class
		{
			var scope = serviceProvider.CreateScope();

			if(typeof(T) == typeof(AppDbContext))
				return scope.ServiceProvider.GetRequiredService<AppDbContext>() as T;

			return scope.ServiceProvider.GetRequiredService<T>();


		}
	}
}
