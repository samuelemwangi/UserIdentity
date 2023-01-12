using System.Net.Mime;

using Microsoft.EntityFrameworkCore;

using UserIdentity;
using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Infrastructure.Security;
using UserIdentity.Infrastructure.Security.Interfaces;
using UserIdentity.Infrastructure.Utilities;
using UserIdentity.Persistence;
using UserIdentity.Persistence.Migrations;
using UserIdentity.Persistence.Settings.Mysql;
using UserIdentity.Presentation.Helpers;
using UserIdentity.Presentation.Helpers.ValidationExceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// MYSQL DB
// Register Configuration
var mysqlSettings =  builder.Configuration.GetSection(nameof(MysqlSettings)).Get<MysqlSettings>();
String connectionString = mysqlSettings.ConnectionString(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// JWT Identity
builder.Services.AddAppAuthentication(builder.Configuration);
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

// JWT Helpers
builder.Services.AddScoped<IJwtFactory, JwtFactory>();
builder.Services.AddScoped<IJwtTokenHandler, JwtTokenHandler>();
builder.Services.AddScoped<IJwtTokenValidator, JwtTokenValidator>();
builder.Services.AddScoped<ITokenFactory, TokenFactory>();

builder.Services.AddScoped<IKeySetFactory, KeySetFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

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