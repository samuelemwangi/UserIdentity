namespace UserIdentity.Application.Interfaces.Security
{
  public interface ITokenFactory
  {
    String GenerateRefreshToken(int size = 32);

    String GenerateOTPToken();
  }


}
