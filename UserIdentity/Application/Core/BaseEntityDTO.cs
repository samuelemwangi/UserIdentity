namespace UserIdentity.Application.Core
{
	public abstract record BaseEntityDTO
	{
		public Guid? Id { get; init; }
		public string? CreatedBy { get; set; }
		public string? CreatedAt { get; set; }
		public string? UpdatedBy { get; set; }
		public string? UpdatedAt { get; set; }
	}
}
