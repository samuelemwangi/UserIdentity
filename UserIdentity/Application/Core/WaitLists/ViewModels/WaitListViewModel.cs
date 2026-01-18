using PolyzenKit.Application.Core;

using UserIdentity.Domain.WaitLists;

namespace UserIdentity.Application.Core.WaitLists.ViewModels;

public record WaitListViewModel : ItemDetailBaseViewModel
{
  public required WaitListDTO WaitList { get; init; }
}
