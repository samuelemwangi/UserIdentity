using System.Linq.Expressions;

using PolyzenKit.Domain.DTO;

namespace UserIdentity.Domain.WaitLists;

public class WaitListDTO : WaitListEntity, IBaseAuditableEntityDTO<long>
{
  public string? AppName { get; set; }

  public static Expression<Func<WaitListEntity, WaitListDTO>> Basic
    => entity => new WaitListDTO
    {
      Id = entity.Id,
      UserEmail = entity.UserEmail,
      AppId = entity.AppId
    };

  private static readonly Func<WaitListEntity, WaitListDTO> _compiledBasic
       = Basic.Compile();

  public static WaitListDTO FromEntityBasic(WaitListEntity entity)
        => _compiledBasic(entity);

  public static Expression<Func<WaitListEntity, WaitListDTO>> Detailed
    => entity => new WaitListDTO
    {
      Id = entity.Id,
      UserEmail = entity.UserEmail,
      AppId = entity.AppId,
      AppName = entity.App.AppName,
      CreatedBy = entity.CreatedBy,
      CreatedAt = entity.CreatedAt,
      UpdatedBy = entity.UpdatedBy,
      UpdatedAt = entity.UpdatedAt
    };

  private static readonly Func<WaitListEntity, WaitListDTO> _compiledDetailed
        = Detailed.Compile();

  public static WaitListDTO FromEntityDetailed(WaitListEntity entity)
        => _compiledDetailed(entity);
}
