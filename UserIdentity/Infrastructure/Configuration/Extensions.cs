using UserIdentity.Application.Exceptions;

namespace UserIdentity.Infrastructure.Configuration
{
	public static class Extensions
	{
		public static string GetEnvironmentVariable(this IConfiguration configuration, string key, string? defaultValue = null)
		{
			var value = configuration[key];
			var envValueExists = !string.IsNullOrEmpty(value);

			if (!envValueExists && defaultValue == null)
			{
				throw new MissingConfigurationException(key);
			}

#pragma warning disable CS8603 // Possible null reference return.
			return envValueExists ? value : defaultValue;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}
}
