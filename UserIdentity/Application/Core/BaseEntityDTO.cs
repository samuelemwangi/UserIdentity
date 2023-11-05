namespace UserIdentity.Application.Core
{
  public abstract record BaseEntityDTO
  {
    public Guid? Id { get; init; }
    public String? CreatedBy { get; set; }
    public String? CreatedAt { get; set; }
    public String? UpdatedBy { get; set; }
    public String? UpdatedAt { get; set; }
  }
}
