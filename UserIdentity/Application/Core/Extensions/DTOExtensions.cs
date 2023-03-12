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
							dto?.LastModifiedBy == loggedInUserId
						 );
		}
	}
}
