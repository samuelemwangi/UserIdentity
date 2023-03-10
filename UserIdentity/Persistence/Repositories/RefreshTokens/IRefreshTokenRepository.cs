using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Repositories.RefreshTokens
{
	public interface IRefreshTokenRepository
	{
		Task<Int32> CreateRefreshTokenAsync(RefreshToken refreshToken);

		Task<RefreshToken?> GetRefreshTokenAsync(String? userId, String? token);

		Task<Int32> UpdateRefreshTokenAsync(RefreshToken refreshToken);
	}
}
