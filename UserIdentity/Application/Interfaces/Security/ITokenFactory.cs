namespace UserIdentity.Application.Interfaces.Security
{
	public interface ITokenFactory
	{
		string GenerateRefreshToken(int size = 32);

		string GenerateOTPToken();
	}


}
