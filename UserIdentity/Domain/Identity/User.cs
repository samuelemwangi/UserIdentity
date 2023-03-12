using UserIdentity.Domain;

namespace UserIdentity.Domain.Identity
{
	public class User : BaseEntity
	{
		public new String? Id { get;  set; }

		public String? FirstName { get;  set; }

		public String? LastName { get; set; }

		public String? EmailConfirmationToken { get; set; }

		public String? ForgotPasswordToken { get; set; }
	}
}
