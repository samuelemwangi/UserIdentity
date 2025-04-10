

using Microsoft.EntityFrameworkCore;

using PolyzenKit.Common.Exceptions;
using PolyzenKit.Common.Utilities;
using PolyzenKit.Persistence.Repositories;

using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Repositories.RegisteredApps;

public class RegisteredAppRepository(
	AppDbContext dbContext
	) : EntityRepository<RegisteredAppEntity, int>(dbContext), IRegisteredAppRepository
{
	private readonly AppDbContext _dbContext = dbContext;

	public async Task<RegisteredAppEntity?> GetEntityByAlternateIdAsync(RegisteredAppEntity entity, QueryCondition queryCondition)
	{
		var appName = ObjectUtil.RequireNonNullValue(entity.AppName, nameof(entity.AppName));

		var existingEntity = await _dbContext.RegisteredApp
			.SingleOrDefaultAsync(x => x.AppName == appName);

		return queryCondition switch
		{
			QueryCondition.MUST_EXIST when existingEntity == null => throw new NoRecordException(appName, nameof(RegisteredAppEntity)),
			QueryCondition.MUST_NOT_EXIST when existingEntity != null => throw new RecordExistsException(appName, nameof(RegisteredAppEntity)),
			_ => existingEntity
		};
	}

	public async Task<List<RegisteredAppDTO>> GetEntityDTOsAsync()
	{
		return await _dbContext.RegisteredApp
			.Where(e => !e.IsDeleted)
			.Select(RegisteredAppDTO.Basic)
			.ToListAsync();
	}
}
