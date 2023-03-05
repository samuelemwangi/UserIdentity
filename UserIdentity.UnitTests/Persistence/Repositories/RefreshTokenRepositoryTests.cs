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
		}

		[Fact]
		public async Task Create_Refresh_Token_Failure_Does_Not_Create_Refresh_Token()
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

			if(result == 1)
			 result = await refreshTokenRepo.CreateRefreshTokenAsync(refreshToken);

			//Assert
			Assert.Equal(0, result);
		}

		[Fact]
		public async Task Get_Existing_Refresh_Token_Returns_Refresh_Token()
		{
			// Arrange
			var context = AppDbContextTestFactory.GetAppDbContext();

			var refreshTokenRepo = new RefreshTokenRepository(context);

			var refreshToken = new RefreshToken
			{
				Id = Guid.NewGuid(),
				UserId = Guid.NewGuid().ToString(),
				RemoteIpAddress = Guid.NewGuid().ToString(),
				CreatedBy = Guid.NewGuid().ToString(),
				Expires = DateTime.UtcNow.AddMinutes(10),
			};

			// Act
			context.RefreshToken.Add(refreshToken);
			await context.SaveChangesAsync();
			var result = await refreshTokenRepo.GetRefreshTokenAsync(refreshToken.UserId, refreshToken.Token);

			// Assert
			Assert.IsType<RefreshToken>(result);
			Assert.Equal(refreshToken.Token, result?.Token);
			Assert.Equal(refreshToken.CreatedBy, result?.CreatedBy);
			Assert.True(result?.Active);
		}

		[Fact]
		public async Task Get_Non_Existing_Refresh_Token_Returns_Null()
		{
			// Arrange
			var context = AppDbContextTestFactory.GetAppDbContext();

			var refreshTokenRepo = new RefreshTokenRepository(context);

			// Act
			var result = await refreshTokenRepo.GetRefreshTokenAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task Update_Existing_Refresh_Token_Updates_Refresh_Token()
		{
			// Arrange
			var context = AppDbContextTestFactory.GetAppDbContext();

			var refreshTokenRepo = new RefreshTokenRepository(context);

			var refreshToken = new RefreshToken
			{
				Id = Guid.NewGuid(),
				UserId = Guid.NewGuid().ToString(),
				RemoteIpAddress = Guid.NewGuid().ToString(),
				CreatedBy = Guid.NewGuid().ToString(),
				Expires = DateTime.UtcNow.AddMinutes(10),
			};


			// Act
			context.RefreshToken.Add(refreshToken);
			context.SaveChanges();
			var updatedRefreshToken = context.RefreshToken.Where(e => e.Id == refreshToken.Id).First();
			updatedRefreshToken.Expires = DateTime.UtcNow.AddMinutes(-10);

			var result = await refreshTokenRepo.UpdateRefreshTokenAsync(updatedRefreshToken);

			// Assert
			Assert.Equal(1, result);
		}

		[Fact]
		public async Task Update_Non_Existing_Refresh_Token_Does_Not_Update_Refresh_Token()
		{
			// Arrange
			var context = AppDbContextTestFactory.GetAppDbContext();

			var refreshTokenRepo = new RefreshTokenRepository(context);

			var refreshToken = new RefreshToken
			{
				Id = Guid.NewGuid(),
				UserId = Guid.NewGuid().ToString(),
				RemoteIpAddress = Guid.NewGuid().ToString(),
				CreatedBy = Guid.NewGuid().ToString(),
				Expires = DateTime.UtcNow.AddMinutes(10),
			};

			var updatedRefreshToken = new RefreshToken
			{
				Id = Guid.NewGuid(),
				Expires = DateTime.UtcNow.AddMinutes(20),
			};

			// Act
			context.RefreshToken.Add(refreshToken);
			await context.SaveChangesAsync();
			var result = await refreshTokenRepo.UpdateRefreshTokenAsync(updatedRefreshToken);

			// Assert
			Assert.Equal(0, result);
		}

	}
}
