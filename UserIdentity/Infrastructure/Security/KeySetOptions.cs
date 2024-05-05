namespace UserIdentity.Infrastructure.Security
{
	public class KeySetOptions
	{
		public string? Alg { get; set; }
		public string? KeyType { get; set; }
		public string? KeyId { get; set; }
		public string KeyProvider { get; set; }
		public string? PrivateKeyPath { get; set; }
		public string? PrivateKeyPassPhrase { get; set; }
		public string? PublicKeyPath { get; set; }
	}
}
