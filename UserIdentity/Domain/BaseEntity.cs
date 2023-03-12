namespace UserIdentity.Domain
{
	public abstract class BaseEntity
	{
		public Guid Id { get; set; }

		public String? CreatedBy { get; set; }

		public DateTime? CreatedDate { get; set; }

		public String? LastModifiedBy { get; set; }

		public DateTime? LastModifiedDate { get; set; }

		public Boolean IsDeleted { get; set; }
	}
}
