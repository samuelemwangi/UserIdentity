using Microsoft.IdentityModel.Tokens;

namespace UserIdentity.Infrastructure.Security
{
	public class JwtIssuerOptions
	{
		public string Issuer { get; init; }

		public string Subject { get; init; }

		public string Audience { get; init; }

		public DateTime NotBefore => DateTime.UtcNow;

		public DateTime IssuedAt => DateTime.UtcNow;

		public TimeSpan ValidFor { get; init; }

		public DateTime Expiration => IssuedAt.Add(ValidFor);

		public Func<string> JtiGenerator => () => Guid.NewGuid().ToString();

		public SigningCredentials SigningCredentials { get; init; }
	}
}
