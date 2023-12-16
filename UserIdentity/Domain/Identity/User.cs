namespace UserIdentity.Domain.Identity
{
	public class User : BaseEntity
	{
		public new string? Id { get; internal set; }

		public string? FirstName { get; internal set; }

		public string? LastName { get; internal set; }

		public string? EmailConfirmationToken { get; internal set; }

		public string? ForgotPasswordToken { get; internal set; }
	}
}
