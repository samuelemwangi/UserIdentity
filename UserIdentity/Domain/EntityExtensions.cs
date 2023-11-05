namespace UserIdentity.Domain
{
  public static class EntityExtensions
  {
    public static void SetAuditFields(this BaseEntity entity, String? userId, DateTime? dateTime)
    {
      entity.CreatedBy = userId;
      entity.CreatedAt = dateTime;
      entity.UpdatedBy = userId;
      entity.UpdatedAt = dateTime;
    }

    public static void UpdateAuditFields(this BaseEntity entity, String? userId, DateTime? dateTime, Boolean IsDeleted = false)
    {
      entity.UpdatedBy = userId;

      if (IsDeleted)
        entity.IsDeleted = true;

      entity.UpdatedAt = dateTime;
    }

  }
}
