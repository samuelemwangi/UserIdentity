using System;

using Microsoft.Extensions.DependencyInjection;

using UserIdentity.Persistence;

namespace UserIdentity.IntegrationTests
{
	internal static class ServiceResolver
	{
	
		public static T ResolveService<T>(IServiceProvider serviceProvider) where T : class
		{
			var scope = serviceProvider.CreateScope();
			return scope.ServiceProvider.GetRequiredService<T>();
		}
	}
}
