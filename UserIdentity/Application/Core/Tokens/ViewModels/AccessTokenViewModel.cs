namespace UserIdentity.Application.Core.Tokens.ViewModels
{
	public record AccessTokenDTO
	{
		public String? Token { get; init; }
		public Int32 ExpiresIn { get; init; }
	}

	public record AccessTokenViewModel
	{
		public AccessTokenDTO? AccessToken { get; init; }
		public String? RefreshToken { get; init; }

	}
}
