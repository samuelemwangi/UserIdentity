using UserIdentity.Infrastructure.Security.Helpers;

using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Security.Helpers
{
	public class ConstantsTests
	{
		[Fact]
		public void JwtClaimIdentifiers_Rol_Returns_Roles()
		{
			Assert.Equal("roles", Constants.Strings.JwtClaimIdentifiers.Rol);
		}

		[Fact]
		public void JwtClaimIdentifiers_Id_Returns_Id()
		{
			Assert.Equal("id", Constants.Strings.JwtClaimIdentifiers.Id);
		}

		[Fact]
		public void JwtClaimIdentifiers_Scope_Returns_Scopes()
		{
			Assert.Equal("scopes", Constants.Strings.JwtClaimIdentifiers.Scope);
		}

		[Fact]
		public void JwtClaims_API_Access_Returns_API_Access()
		{
			Assert.Equal("api_access", Constants.Strings.JwtClaims.ApiAccess);
		}
	}
}

