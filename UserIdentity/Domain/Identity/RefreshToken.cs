namespace UserIdentity.Domain.Identity
{
	public class RefreshToken : BaseEntity
	{
		public String? Token { get; internal set; }

		public DateTime Expires { get; internal set; }

		public String? UserId { get; internal set; }

		public Boolean? Active => DateTime.UtcNow <= Expires;

		public String? RemoteIpAddress { get; internal set; }
	}
}
