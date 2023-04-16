namespace UserIdentity.Application.Core
{
	public abstract record BaseEntityDTO
	{
		public Guid? Id { get; init; }
		public string? CreatedBy { get; internal set; }
		public string? CreatedAt { get; internal set; }
		public string? UpdatedBy { get; internal set; }
		public string? UpdatedAt { get; internal set; }
	}
}
