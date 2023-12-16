namespace UserIdentity.Application.Core.Tokens.ViewModels
{
	public record AccessTokenDTO
	{
		public string? Token { get; init; }
		public int ExpiresIn { get; init; }
	}

	public record AccessTokenViewModel
	{
		public AccessTokenDTO? AccessToken { get; init; }
		public string? RefreshToken { get; init; }

	}
}
