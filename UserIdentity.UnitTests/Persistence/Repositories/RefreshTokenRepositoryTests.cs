using System;
using System.Linq;
using System.Threading.Tasks;

using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RefreshTokens;

using Xunit;

namespace UserIdentity.UnitTests.Persistence.Repositories
{
	public class RefreshTokenRepositoryTests
	{

		[Fact]
		public async Task Create_Refresh_Token_Saves_New_Refresh_Token()
		{
			// Arrange
			var context = AppDbContextTestFactory.GetAppDbContext();
			var refreshTokenRepo = new RefreshTokenRepository(context);

			var refreshToken = new RefreshToken
			{
				Id = Guid.NewGuid(),
				UserId = Guid.NewGuid().ToString(),
				RemoteIpAddress = Guid.NewGuid().ToString()
			};

			// Act
			var result = await refreshTokenRepo.CreateRefreshTokenAsync(refreshToken);

			//Assert
			Assert.Equal(1, result);
			Assert.Equal(refreshToken.Id, context.RefreshToken.FirstOrDefault()?.Id);
			Assert.Equal(refreshToken.UserId, context.RefreshToken.FirstOrDefault()?.UserId);
			Assert.Equal(refreshToken.RemoteIpAddress, context.RefreshToken.FirstOrDefault()?.RemoteIpAddress);
		}
	}
}
