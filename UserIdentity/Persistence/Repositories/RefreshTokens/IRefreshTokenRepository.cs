using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Repositories.RefreshTokens
{
	public interface IRefreshTokenRepository
	{
		Task<int> CreateRefreshTokenAsync(RefreshToken refreshToken);

		Task<RefreshToken?> GetRefreshTokenAsync(string? userId, string? token);

		Task<int> UpdateRefreshTokenAsync(RefreshToken refreshToken);
	}
}
