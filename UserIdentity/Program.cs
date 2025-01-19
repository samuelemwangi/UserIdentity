using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;

using PolyzenKit.Infrastructure.Security.KeyProviders;
using PolyzenKit.Infrastructure.Security.KeySets;
using PolyzenKit.Presentation;
using PolyzenKit.Presentation.Middlewares;
using PolyzenKit.Presentation.ValidationHelpers;

using UserIdentity;
using UserIdentity.Persistence;
using UserIdentity.Persistence.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.AddAppSerilog();

// Add MYSQL DB
string connectionString = builder.Configuration.GetAppMysqlConnectionString();
builder.Services.AddDbContext<DbContext, AppDbContext>(options =>
{
	options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)).UseSnakeCaseNamingConvention();
});

// Repositories
builder.Services.AddAppRepositories();

// Command and Query Handlers
builder.Services.AddAppCommandAndQueryHandlers();

// Controllers
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
	options.InvalidModelStateResponseFactory = context =>
	{
		var result = new ValidationFailedResult(context.ModelState);
		result.ContentTypes.Add(MediaTypeNames.Application.Json);
		return result;
	};
}).AddJsonOptions(options =>
{
	options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Utilities e.g for Time, String, Logging 
builder.Services.AddAppDateTimeStringLogHelpers();

// Authentication Identity
builder.Services.AddAppAuthentication(
	builder.Configuration,
	true,
	(config) => new FileSystemKeyProvider(),
	(options, keyProvider) => new EdDSAKeySetFactory(options, keyProvider)
);

// Authorization
builder.Services.AddAppAuthorization(builder.Configuration);

// Identity
builder.Services.AddAppIdentity();

// Api Key Settings
builder.Services.AddAppApiKeySettings(builder.Configuration);

// Cors Policy
var corsPolicyName = builder.Services.AddAppCorsPolicy(builder.Configuration);

// build the app
var app = builder.Build();

// Use Cors
app.UseCors(corsPolicyName);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	IdentityModelEventSource.ShowPII = true;
	IdentityModelEventSource.LogCompleteSecurityArtifact = true;
}

// Use Api Key Middleware
app.UseMiddleware<ApiKeyMiddleware>();

// Use Request Id Middleware
app.UseMiddleware<RequestIdMiddleware>();

// Use Exception Middleware
app.UseMiddleware<ExceptionMiddleware>();

// Use HTTPS
app.UseHttpsRedirection();

// Use AuthN
app.UseAuthentication();

// Use AuthZ
app.UseAuthorization();

// Use Endpoints
app.MapControllers();

// Migrate And Seed DB
DbInitializer.InitializeDb(app);
app.AppSeedEntityNamesData();

// Run the app
app.Run();

// use this for testing 
public partial class Program { }
