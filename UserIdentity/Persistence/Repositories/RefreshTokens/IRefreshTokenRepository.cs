using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Repositories.RefreshTokens;

public interface IRefreshTokenRepository
{
	Task<int> CreateRefreshTokenAsync(RefreshTokenEntity refreshToken);

	Task<RefreshTokenEntity?> GetRefreshTokenAsync(string userId, string token);

	Task<int> UpdateRefreshTokenAsync(RefreshTokenEntity refreshToken);

	Task DeleteRefreshTokenAsync(string userId);
}
