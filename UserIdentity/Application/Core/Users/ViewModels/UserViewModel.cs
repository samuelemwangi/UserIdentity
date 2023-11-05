namespace UserIdentity.Application.Core.Users.ViewModels
{
  public record UserDTO : BaseEntityDTO
  {
    public new String? Id { get; init; }
    public String? FullName { get; init; }
    public String? UserName { get; init; }
    public String? Email { get; init; }

  }

  public record UserViewModel : ItemDetailBaseViewModel
  {
    public UserDTO? User { get; init; }
  }
}
