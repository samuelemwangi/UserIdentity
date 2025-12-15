using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using PolyzenKit.Domain.AppEntities;
using PolyzenKit.Domain.RegisteredApps;

using Respawn;

using UserIdentity.Persistence;
using UserIdentity.Persistence.Infrastructure;

using Xunit;
using Xunit.Abstractions;

namespace UserIdentity.IntegrationTests.Presentation.Controllers;

[Collection("Integration Tests")]
public class BaseControllerTests : IDisposable, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    private readonly Func<Task> _resetDatabase;

    protected readonly ITestOutputHelper _outputHelper;
    protected readonly HttpClient _httpClient;
    protected readonly IServiceProvider _serviceProvider;

    protected readonly AppDbContext _appDbContext;
    protected readonly UserManager<IdentityUser> _userManager;
    protected readonly RoleManager<IdentityRole> _roleManager;

    public BaseControllerTests(TestingWebAppFactory testingWebAppFactory, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _httpClient = testingWebAppFactory.CreateClient();

        _scope = testingWebAppFactory.Services.CreateScope();
        _serviceProvider = _scope.ServiceProvider;

        _appDbContext = _serviceProvider.GetRequiredService<AppDbContext>();
        _userManager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        _roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        _resetDatabase = ResolveResetDatabaseFunc();
    }

    private Func<Task> ResolveResetDatabaseFunc()
    {
        return async () =>
        {
            var dbConnection = _appDbContext.Database.GetDbConnection();

            await dbConnection.OpenAsync();

            var respawner = await Respawner.CreateAsync(dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.MySql,
                TablesToIgnore = [_appDbContext.GetTableName<AppEntity>()!, _appDbContext.GetTableName<RegisteredAppEntity>()!]
            });

            await respawner.ResetAsync(dbConnection);
        };
    }

    public void Dispose()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task InitializeAsync() => await _resetDatabase();

    public Task DisposeAsync() => Task.CompletedTask;
}
