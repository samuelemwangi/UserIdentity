using Microsoft.EntityFrameworkCore;

using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Repositories.RefreshTokens
{
	public class RefreshTokenRepository : IRefreshTokenRepository
	{
		private readonly AppDbContext _appDbContext;

		public RefreshTokenRepository(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}
		public async Task<Int32> CreateRefreshTokenAsync(RefreshToken refreshToken)
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

		public async Task<RefreshToken?> GetRefreshTokenAsync(String? userId, String? token)
		{
			var refreshToken = await _appDbContext.RefreshToken
				.Where(e => e.UserId == userId && e.Token == token)
				.FirstOrDefaultAsync();

			return refreshToken;
		}

		public async Task<Int32> UpdateRefreshTokenAsync(RefreshToken refreshToken)
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
	}
}
