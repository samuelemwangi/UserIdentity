using Microsoft.EntityFrameworkCore;

using PolyzenKit.Application.Enums;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Common.Utilities;
using PolyzenKit.Persistence.Repositories;

using UserIdentity.Common;
using UserIdentity.Domain.InviteCodes;

namespace UserIdentity.Persistence.Repositories.InviteCodes;

public class InviteCodeRepository(
  AppDbContext dbContext
 ) : EntityRepository<InviteCodeEntity, long>(dbContext), IInviteCodeRepository
{
  private readonly AppDbContext _dbContext = dbContext;
  public async Task<InviteCodeEntity?> GetEntityByAlternateIdAsync(InviteCodeEntity entity, QueryCondition queryCondition)
  {
    var userEmail = ObjectUtil.RequireNonNullValue(entity.UserEmail, nameof(entity.UserEmail));

    var existingEntity = await _dbContext.InviteCode
      .Include(e => e.App)
      .SingleOrDefaultAsync(e => e.UserEmail == userEmail);

    return queryCondition switch
    {
      QueryCondition.MUST_EXIST when existingEntity == null => throw new NoRecordException(userEmail, nameof(InviteCodeEntity)),
      QueryCondition.MUST_NOT_EXIST when existingEntity != null => throw new RecordExistsException(userEmail, nameof(InviteCodeEntity)),
      _ => existingEntity
    };
  }

  public async Task<InviteCodeDTO> GetEntityDTOByIdAsync(long id)
  {
    return await _dbContext.InviteCode
      .Where(e => e.Id == id && !e.IsDeleted)
      .Select(InviteCodeDTO.Detailed)
      .SingleOrDefaultAsync() ?? throw new NoRecordException($"{id}", EntityTypes.INVITE_CODE.Description());
  }
}
