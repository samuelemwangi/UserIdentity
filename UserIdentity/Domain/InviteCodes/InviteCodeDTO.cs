using System.Linq.Expressions;

using PolyzenKit.Domain.DTO;

namespace UserIdentity.Domain.InviteCodes;

public class InviteCodeDTO : InviteCodeEntity, IBaseAuditableEntityDTO<long>
{
  public string? AppName { get; set; }

  public static Expression<Func<InviteCodeEntity, InviteCodeDTO>> Basic
    => entity => new InviteCodeDTO
    {
      Id = entity.Id,
      InviteCode = entity.InviteCode,
      UserEmail = entity.UserEmail,
      AppId = entity.AppId
    };

  private static readonly Func<InviteCodeEntity, InviteCodeDTO> _compiledBasic
       = Basic.Compile();

  public static InviteCodeDTO FromEntityBasic(InviteCodeEntity entity)
        => _compiledBasic(entity);

  public static Expression<Func<InviteCodeEntity, InviteCodeDTO>> Detailed
    => entity => new InviteCodeDTO
    {
      Id = entity.Id,
      InviteCode = entity.InviteCode,
      UserEmail = entity.UserEmail,
      AppId = entity.AppId,
      AppName = entity.App.AppName,
      CreatedBy = entity.CreatedBy,
      CreatedAt = entity.CreatedAt,
      UpdatedBy = entity.UpdatedBy,
      UpdatedAt = entity.UpdatedAt
    };

  private static readonly Func<InviteCodeEntity, InviteCodeDTO> _compiledDetailed
        = Detailed.Compile();

  public static InviteCodeDTO FromEntityDetailed(InviteCodeEntity entity)
        => _compiledDetailed(entity);
}
