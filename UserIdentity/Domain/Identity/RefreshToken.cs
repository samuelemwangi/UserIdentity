namespace UserIdentity.Domain.Identity
{
	public class RefreshToken : BaseEntity
	{
		public String? Token { get;  set; }

		public DateTime Expires { get;  set; }

		public String? UserId { get; set; }

		public Boolean? Active => DateTime.UtcNow <= Expires;

		public String? RemoteIpAddress { get; set; }
	}
}
