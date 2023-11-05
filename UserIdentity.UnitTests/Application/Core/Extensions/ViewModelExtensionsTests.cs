using UserIdentity.Application.Core;
using UserIdentity.Application.Core.Extensions;
using UserIdentity.Application.Enums;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Extensions
{
  public record TestBaseVM : BaseViewModel
  {

  }

  public record TestItemDetailBaseVM : ItemDetailBaseViewModel
  {

  }


  public record TestItemsBaseVM : ItemsBaseViewModel
  {

  }

  public class ViewModelExtensionsTests
  {
    [Fact]
    public void Resolve_Request_Status_Updates_VM_Status()
    {
      // Arrange
      var vm = new TestBaseVM();

      // Act
      vm.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEM_SUCCESSFUL);

      // Assert
      Assert.Equal(RequestStatus.SUCCESSFUL.GetDisplayName(), vm.RequestStatus);
      Assert.Equal(ItemStatusMessage.FETCH_ITEM_SUCCESSFUL.GetDisplayName(), vm.StatusMessage);
    }

    [Fact]
    public void Resolve_StatusMessage_Status_Updates_VM_StatusMessage()
    {
      // Arrange
      var vm = new TestBaseVM();
      var customMesage = "Failed but looks like success";

      // Act
      vm.ResolveRequestStatus(RequestStatus.FAILED, ItemStatusMessage.FETCH_ITEM_SUCCESSFUL, customMesage);

      // Assert
      Assert.Equal(RequestStatus.FAILED.GetDisplayName(), vm.RequestStatus);
      Assert.Equal(customMesage, vm.StatusMessage);
    }

    [Fact]
    public void Resolve_EditDelete_Rights_Updates_EditDelete_Rights()
    {
      // Arrange
      var vm = new TestItemDetailBaseVM();
      var vm2 = new TestItemDetailBaseVM();

      var targetEntity = "TestEntity";
      var userRoleClaims = $"{targetEntity}:edit,{targetEntity}:delete";

      // Act
      vm.ResolveEditDeleteRights(userRoleClaims, targetEntity);
      vm2.ResolveEditDeleteRights("", targetEntity);

      // Assert
      Assert.True(vm.EditEnabled);
      Assert.True(vm.DeleteEnabled);

      Assert.False(vm2.EditEnabled);
      Assert.False(vm2.DeleteEnabled);
    }

    [Fact]
    public void Resolve_CreateDownload_Rights_Updates_CreateDownload_Rights()
    {
      // Arrange
      var vm = new TestItemsBaseVM();
      var vm2 = new TestItemsBaseVM();

      var targetEntity = "TestEntity";
      var userRoleClaims = $"{targetEntity}:create,{targetEntity}:download";

      // Act
      vm.ResolveCreateDownloadRights(userRoleClaims, targetEntity);
      vm2.ResolveCreateDownloadRights("", targetEntity);

      // Assert
      Assert.True(vm.CreateEnabled);
      Assert.True(vm.DownloadEnabled);

      Assert.False(vm2.CreateEnabled);
      Assert.False(vm2.DownloadEnabled);
    }

  }
}

