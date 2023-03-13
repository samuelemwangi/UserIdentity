using UserIdentity.Domain;

namespace UserIdentity.Domain.Identity
{
	public class User : BaseEntity
	{
		public new String? Id { get; internal set; }

		public String? FirstName { get; internal set; }

		public String? LastName { get; internal set; }

		public String? EmailConfirmationToken { get; internal set; }

		public String? ForgotPasswordToken { get; internal set; }
	}
}
