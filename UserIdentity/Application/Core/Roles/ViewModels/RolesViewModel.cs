namespace UserIdentity.Application.Core.Roles.ViewModels
{

  public record RolesViewModel : ItemsBaseViewModel
  {
    public ICollection<RoleDTO> Roles { get; init; }
  }

  public record UserRolesViewModel : ItemsBaseViewModel
  {
    public ICollection<String> UserRoles { get; init; }
  }
}
