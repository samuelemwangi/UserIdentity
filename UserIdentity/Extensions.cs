using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using UserIdentity.Infrastructure.Security;
using UserIdentity.Persistence;

namespace UserIdentity
{
	public static class Extensions
	{
		public static void AddCommandAndQueryHandlers(this IServiceCollection services)
		{
			// Register all command and query handlers
			foreach (var executingType in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (executingType.IsClass && !executingType.IsAbstract && (executingType.Name.Contains("QueryHandler") || executingType.Name.Contains("CommandHandler")))
				{
					services.AddScoped(executingType);
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
			var keySetFactory = new KeySetFactory(configuration);
			var signingKey = keySetFactory.GetSigningKey();
			var signingCredentials = new SigningCredentials(signingKey, keySetFactory.GetAlgorithm());

			// Key Id 
			signingCredentials.Key.KeyId = keySetFactory.GetKeyId();

			// JWT wire up
			var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

			// Configure JwtIssuerOptions
			services.Configure<JwtIssuerOptions>(options =>
			{
				options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
				options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
				options.ValidFor = TimeSpan.FromSeconds(Double.Parse(jwtAppSettingOptions[nameof(JwtIssuerOptions.ValidFor)]));
				options.SigningCredentials = signingCredentials;
			});


			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

				ValidateAudience = true,
				ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

				ValidateIssuerSigningKey = true,
				IssuerSigningKey = signingKey,

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
				configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
				configureOptions.TokenValidationParameters = tokenValidationParameters;
				configureOptions.SaveToken = true;


				configureOptions.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
						{
							context.Response.Headers.Add("X-Token-Expired", "true");
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
