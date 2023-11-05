using System.Security.Cryptography;

using UserIdentity.Application.Interfaces.Security;

namespace UserIdentity.Infrastructure.Security
{
  public class TokenFactory : ITokenFactory
  {
    public String GenerateRefreshToken(int size = 32)
    {
      var randomNumber = new byte[size];
      using var rng = RandomNumberGenerator.Create();

      rng.GetBytes(randomNumber);

      return Convert.ToBase64String(randomNumber);
    }
    public String GenerateOTPToken()
    {
      throw new NotImplementedException();
    }
  }


}
