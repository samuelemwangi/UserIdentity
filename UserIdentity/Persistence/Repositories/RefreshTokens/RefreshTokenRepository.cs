﻿using Microsoft.EntityFrameworkCore;

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
		public async Task<int> CreateRefreshTokenAsync(RefreshToken refreshToken)
		{
			_appDbContext.RefreshToken?.Add(refreshToken);

			return await _appDbContext.SaveChangesAsync();
		}

		public async Task<RefreshToken?> GetRefreshTokenAsync(string? userId, string? token)
		{
			var refreshToken = await _appDbContext.RefreshToken
				.Where(e => e.UserId == userId && e.Token == token)
				.FirstOrDefaultAsync();

			return refreshToken;
		}

		public async Task<int> UpdateRefreshTokenAsync(RefreshToken refreshToken)
		{
			_appDbContext.RefreshToken?.Update(refreshToken);
			return await _appDbContext.SaveChangesAsync();
		}
	}
}
