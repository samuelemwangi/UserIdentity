using System.Net.Mime;
using System.Text.Json.Serialization;

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

// Serilog
builder.AddAppSerilog();

// DB 
builder.Services.AddAppMysql<AppDbContext>(builder.Configuration);

// Identity
builder.Services.AddAppIdentity<AppDbContext>(builder.Configuration);

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

// Api Key Settings
builder.Services.AddAppApiKeySettings(builder.Configuration);

// Cors Policy
var corsPolicyName = builder.Services.AddAppCorsPolicy(builder.Configuration);

// Health Checks
builder.Services.AddHealthChecks();

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

// Map Health Check Endpoint
app.MapHealthChecks("/health");

// Migrate And Seed DB
DbInitializer.InitializeDb(app);
app.AppSeedEntityNamesData();

// Run the app
app.Run();

// use this for testing 
public partial class Program { }
