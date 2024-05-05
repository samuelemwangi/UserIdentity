using UserIdentity.Application.Exceptions;

namespace UserIdentity.Infrastructure.Configuration
{
	public static class Extensions
	{
		public static string GetEnvironmentVariable(this IConfiguration configuration, string key)
		{
			var value = configuration[key];
			if (string.IsNullOrEmpty(value))
			{
				throw new MissingConfigurationException(key);
			}
			return value;
		}
	}
}
