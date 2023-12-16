namespace UserIdentity.Domain.Identity
{
	public class RefreshToken : BaseEntity
	{
		public string? Token { get; internal set; }

		public DateTime Expires { get; internal set; }

		public string? UserId { get; internal set; }

		public bool? Active => DateTime.UtcNow <= Expires;

		public string? RemoteIpAddress { get; internal set; }
	}
}
