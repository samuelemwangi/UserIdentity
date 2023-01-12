namespace UserIdentity.Application.Core
{
	public abstract record BaseEntityDTO
	{
		public Guid? Id { get; init; }
		public string? CreatedBy { get; init; }
		public string? CreatedDate { get; init; }
		public string? LastModifiedBy { get; init; }
		public string? LastModifiedDate { get; init; }
	}
}
