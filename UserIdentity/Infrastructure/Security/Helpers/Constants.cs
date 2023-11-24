namespace UserIdentity.Infrastructure.Security.Helpers
{
	public static class Constants
	{
		public static class Strings
		{
			public static class JwtClaimIdentifiers
			{
				public const String Rol = "roles", Id = "id", Scope = "scopes";
			}

			public static class JwtClaims
			{
				public const String ApiAccess = "api_access";
			}
		}
	}
}
