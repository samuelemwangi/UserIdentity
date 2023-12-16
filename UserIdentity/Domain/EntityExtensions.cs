namespace UserIdentity.Domain
{
	public static class EntityExtensions
	{
		public static void SetAuditFields(this BaseEntity entity, string? userId, DateTime? dateTime)
		{
			entity.CreatedBy = userId;
			entity.CreatedAt = dateTime;
			entity.UpdatedBy = userId;
			entity.UpdatedAt = dateTime;
		}

		public static void UpdateAuditFields(this BaseEntity entity, string? userId, DateTime? dateTime, bool IsDeleted = false)
		{
			entity.UpdatedBy = userId;

			if (IsDeleted)
				entity.IsDeleted = true;

			entity.UpdatedAt = dateTime;
		}

	}
}
