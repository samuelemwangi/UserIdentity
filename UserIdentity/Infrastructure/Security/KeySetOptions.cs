namespace UserIdentity.Infrastructure.Security
{
	public class KeySetOptions
	{
		public string? Alg { get; set; }
		public string? KeyType { get; set; }
		public string? KeyId { get; set; }
		public string? SecretKey { get; set; }
	}
}
