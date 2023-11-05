namespace UserIdentity.Domain
{
  public abstract class BaseEntity
  {
    public Guid Id { get; internal set; }

    public String? CreatedBy { get; internal set; }

    public DateTime? CreatedAt { get; internal set; }

    public String? UpdatedBy { get; internal set; }

    public DateTime? UpdatedAt { get; internal set; }

    public Boolean IsDeleted { get; internal set; }
  }
}
