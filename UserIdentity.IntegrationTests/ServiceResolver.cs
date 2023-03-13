using System;

using Microsoft.Extensions.DependencyInjection;

using UserIdentity.Persistence;

namespace UserIdentity.IntegrationTests
{
    internal static class ServiceResolver
    {
        public static AppDbContext ResolveDBContext(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();

            return scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }
    }
}
