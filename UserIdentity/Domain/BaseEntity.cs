namespace UserIdentity.Domain
{
	public abstract class BaseEntity
	{
		public Guid Id { get; internal set; }

		public String? CreatedBy { get; internal set; }

		public DateTime? CreatedDate { get; internal set; }

		public String? LastModifiedBy { get; internal set; }

		public DateTime? LastModifiedDate { get; internal set; }

		public Boolean IsDeleted { get; internal set; }
	}
}
