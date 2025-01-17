using System.Net.Mime;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;

using PolyzenKit.Application.Interfaces;
using PolyzenKit.Infrastructure.Security.KeyProviders;
using PolyzenKit.Infrastructure.Security.KeySets;
using PolyzenKit.Infrastructure.Utilities;
using PolyzenKit.Persistence.Settings;
using PolyzenKit.Presentation;
using PolyzenKit.Presentation.Helpers;
using PolyzenKit.Presentation.ValidationHelpers;

using Serilog;

using UserIdentity;
using UserIdentity.Persistence;
using UserIdentity.Persistence.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.AddSerilog();


// Add services to the container.

// MYSQL DB
// Register Configuration
var mysqlSetting = builder.Configuration.GetSection(nameof(MysqlSettings)).Get<MysqlSettings>()!;
string connectionString = mysqlSetting.ConnectionString(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)).UseSnakeCaseNamingConvention();
});

// Authentication, Authorization and Identity
builder.Services.AddAppAuthentication(
	builder.Configuration,
	(config) => new FileSystemKeyProvider(),
	(options, keyProvider) => new EdDSAKeySetFactory(options, keyProvider)
);
builder.Services.AddAppAuthorization();
builder.Services.AddAppIdentity();

// Controllers
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
	options.InvalidModelStateResponseFactory = context =>
	{
		var result = new ValidationFailedResult(context.ModelState);
		result.ContentTypes.Add(MediaTypeNames.Application.Json);
		return result;
	};
});

// Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Command and Query Handlers
builder.Services.AddCommandAndQueryHandlers();

// Repositories
builder.Services.AddRepositories();

// Utilities e.g for Time, String 
builder.Services.AddScoped<IMachineDateTime, MachineDateTime>();
builder.Services.AddScoped<IStringHelper, StringHelper>();
builder.Services.AddScoped(typeof(ILogHelper<>), typeof(LogHelper<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	IdentityModelEventSource.ShowPII = true;
	IdentityModelEventSource.LogCompleteSecurityArtifact = true;
}

// Extract Request Id
app.UseMiddleware<RequestIdMiddleware>();

// Handle Exceptions
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Migrate DB
MigrationData.MigrateDb(app);
app.Run();

// use this for testing 
public partial class Program { }
