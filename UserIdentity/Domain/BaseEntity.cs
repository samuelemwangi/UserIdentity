namespace UserIdentity.Domain
{
	public abstract class BaseEntity
	{
		public Guid Id { get; internal set; }

		public string? CreatedBy { get; internal set; }

		public DateTime? CreatedAt { get; internal set; }

		public string? UpdatedBy { get; internal set; }

		public DateTime? UpdatedAt { get; internal set; }

		public bool IsDeleted { get; internal set; }
	}
}
