using Microsoft.IdentityModel.Tokens;

namespace UserIdentity.Infrastructure.Security
{
	public class JwtIssuerOptions
	{
		//iss -> Issuer claim identifies the princcipal that issued the JWT 
		public string? Issuer { get; set; }

		//sub -> Subject identifies the subject of the JWT
		public string? Subject { get; set; }

		//aud -> The recipients of the JWT
		public string? Audience { get; set; }

		//nbf -> JWT not valid before this time
		public DateTime NotBefore => DateTime.UtcNow;

		//iat -> Time JWT was issued at 
		public DateTime IssuedAt => DateTime.UtcNow;

		//Timespan the JWT is valid for
		public TimeSpan ValidFor { get; set; }
		//= TimeSpan.FromMinutes(1);

		//exp -> The Time after which JWT must not be accepted for processing
		public DateTime Expiration => IssuedAt.Add(ValidFor);


		// "jti" (JWT ID) Claim (default ID is a GUID)
		// add getter and setter for JtiGenerator
		public Func<Task<string>> JtiGenerator =>
			() => Task.FromResult(Guid.NewGuid().ToString());

		public SigningCredentials? SigningCredentials { get; set; }
	}
}
