using UserIdentity.Domain;

namespace UserIdentity.Application.Core.Extensions
{
	public static class DTOExtensions
	{
		public static bool OwnedByLoggedInUser(this BaseEntityDTO? dto, String? loggedInUserId)
		{
			return (dto != null) &&
						 (loggedInUserId != null) &&
						 (
							dto?.CreatedBy == loggedInUserId ||
							dto?.UpdatedBy == loggedInUserId
						 );
		}

		public static void SetDTOAuditFields(this BaseEntityDTO dto, BaseEntity entity, Func<DateTime?, String> resolveDateTime)
		{
			dto.CreatedBy = entity.CreatedBy;
			dto.CreatedAt = resolveDateTime(entity.CreatedAt);
			dto.UpdatedBy = entity.UpdatedBy;
			dto.UpdatedAt = resolveDateTime(entity.UpdatedAt);
		}
	}
}
