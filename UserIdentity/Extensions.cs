using System.Reflection;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using UserIdentity.Infrastructure.Configuration;
using UserIdentity.Infrastructure.Security;
using UserIdentity.Infrastructure.Utilities;
using UserIdentity.Persistence;

namespace UserIdentity
{
	public static class Extensions
	{
		public static void AddCommandAndQueryHandlers(this IServiceCollection services)
		{
			// Add Query Handlers
			foreach (var executingType in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (executingType.IsClass && !executingType.IsAbstract && executingType.Name.Contains("QueryHandler"))
				{

					var executingTypeInterfaces = executingType.GetInterfaces().Where(x => x.Name.Contains("QueryHandler"));
					if (executingTypeInterfaces.Any())
					{
						foreach (var executingInterface in executingTypeInterfaces)
							services.AddScoped(executingInterface, executingType);
					}

				}
			}

			// Add Command Handlers
			foreach (var executingType in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (executingType.IsClass && !executingType.IsAbstract && executingType.Name.Contains("CommandHandler"))
				{

					var executingTypeInterfaces = executingType.GetInterfaces().Where(x => x.Name.Contains("CommandHandler"));
					if (executingTypeInterfaces.Any())
					{
						foreach (var executingInterface in executingTypeInterfaces)
							services.AddScoped(executingInterface, executingType);
					}

				}
			}

		}

		public static void AddRepositories(this IServiceCollection services)
		{
			foreach (var executingType in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (executingType.IsClass && !executingType.IsAbstract && executingType.Name.Contains("Repository"))
				{

					var executingTypeInterfaces = executingType.GetInterfaces().Where(x => x.Name.Contains("Repository"));
					if (executingTypeInterfaces.Any())
						services.AddScoped(executingTypeInterfaces.First(), executingType);

				}
			}
		}

		public static void AddAppAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			// Signing Credentials
			var keySetFactory = new KeySetFactory(configuration, new FileSystemKeyProvider());
			var algorithm = keySetFactory.GetAlgorithm();
			var signingKey = keySetFactory.GetSigningKeyAsync().Result;
			var verificationKey = keySetFactory.GetVerificationKeyAsync().Result;

			var signingCredentials = new SigningCredentials(signingKey, algorithm);

			// JWT wire up
			var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

			var issuer = configuration.GetEnvironmentVariable(jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)] ?? "APP_ISSUER"); 

			var audience = configuration.GetEnvironmentVariable(jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)] ?? "APP_AUDIENCE"); 

			var validForString = configuration.GetEnvironmentVariable(jwtAppSettingOptions[nameof(JwtIssuerOptions.ValidFor)] ?? "APP_VALID_FOR"); 

			var validFor = 15.0d;

			if (double.TryParse(validForString, out var validForDouble))
				validFor = validForDouble;


			// Configure JwtIssuerOptions
			services.Configure<JwtIssuerOptions>(options =>
			{
				options.Issuer = issuer;
				options.Audience = audience;
				options.ValidFor = TimeSpan.FromSeconds(validFor);
				options.SigningCredentials = signingCredentials;
			});

			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = issuer,

				ValidateAudience = true,
				ValidAudience = audience,

				ValidateIssuerSigningKey = true,
				IssuerSigningKey = verificationKey,

				RequireExpirationTime = false,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			};


			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

			}).AddJwtBearer(configureOptions =>
			{
				configureOptions.ClaimsIssuer = issuer;
				configureOptions.TokenValidationParameters = tokenValidationParameters;
				configureOptions.SaveToken = true;


				configureOptions.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
						{
							context.Response.Headers["X-Token-Expired"] = "true";
						}
						return Task.CompletedTask;
					}
				};
			});

		}

		public static void AddAppAuthorization(this IServiceCollection services)
		{
			// api user claim policy
			services.AddAuthorization();
		}

		public static void AddAppIdentity(this IServiceCollection services)
		{

			// add identity
			var identityBuilder = services.AddIdentityCore<IdentityUser>(o =>
			{
				// configure identity options
				o.Password.RequireDigit = true;
				o.Password.RequireLowercase = false;
				o.Password.RequireUppercase = false;
				o.Password.RequireNonAlphanumeric = false;
				o.Password.RequiredLength = 4;
			});

			identityBuilder.AddRoles<IdentityRole>();

			identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole), identityBuilder.Services);
			identityBuilder.AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

		}
	}
}
