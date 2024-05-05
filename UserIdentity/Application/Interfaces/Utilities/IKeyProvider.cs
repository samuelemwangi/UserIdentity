namespace UserIdentity.Application.Interfaces.Utilities
{
	public interface IKeyProvider
	{
		Task<string> GetKeyAsync(string keyName);
	}
}
