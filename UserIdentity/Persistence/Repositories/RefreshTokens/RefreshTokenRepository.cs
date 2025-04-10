using Microsoft.EntityFrameworkCore;

using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Repositories.RefreshTokens;

public class RefreshTokenRepository(
	AppDbContext appDbContext
	) : IRefreshTokenRepository
{
	private readonly AppDbContext _appDbContext = appDbContext;

	public async Task<int> CreateRefreshTokenAsync(RefreshTokenEntity refreshToken)
	{
		try
		{
			_appDbContext.RefreshToken?.Add(refreshToken);

			return await _appDbContext.SaveChangesAsync();
		}
		catch (Exception)
		{
			return 0;
		}
	}

	public async Task<RefreshTokenEntity?> GetRefreshTokenAsync(string userId, string token)
	{
		var refreshToken = await _appDbContext.RefreshToken
			.Where(e => e.UserId == userId && e.Token == token && !e.IsDeleted)
			.FirstOrDefaultAsync();

		return refreshToken;
	}

	public async Task<int> UpdateRefreshTokenAsync(RefreshTokenEntity refreshToken)
	{
		try
		{
			_appDbContext.RefreshToken?.Update(refreshToken);
			return await _appDbContext.SaveChangesAsync();
		}
		catch (Exception)
		{
			return 0;
		}
	}

	public async Task DeleteRefreshTokenAsync(string userId)
	{
		if ((await _appDbContext.RefreshToken.Where(e => e.UserId == userId).ExecuteDeleteAsync()) < 1)
			throw new InvalidOperationException($"deleting user identified with {userId}");
	}
}
