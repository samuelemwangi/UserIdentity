using System;
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

    public class ViewModelExtensionsTest
    {
        [Fact]
        public void Resolve_Request_Status_Updates_VM_Status()
        {
            TestBaseVM vm = new();

            vm.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

            Assert.Equal(RequestStatus.SUCCESSFUL.GetDisplayName(), vm.RequestStatus);
            Assert.Equal(ItemStatusMessage.SUCCESS.GetDisplayName(), vm.StatusMessage);
        }

        [Fact]
        public void Resolve_StatusMessage_Status_Updates_VM_StatusMessage()
        {
            TestBaseVM vm = new();
            String customMesage = "Failed but looks like success";

            vm.ResolveRequestStatus(RequestStatus.FAILED, ItemStatusMessage.SUCCESS, customMesage);

            Assert.Equal(RequestStatus.FAILED.GetDisplayName(), vm.RequestStatus);
            Assert.Equal(customMesage, vm.StatusMessage);
        }

        [Fact]
        public void Resolve_EditDelete_Rights_Updates_EditDelete_Rights()
        {
            TestItemDetailBaseVM vm = new();
            TestItemDetailBaseVM vm2 = new();

            String targetEntity = "TestEntity";
            String userRoleClaims = $"{targetEntity}:edit,{targetEntity}:delete"; 

            vm.ResolveEditDeleteRights(userRoleClaims,targetEntity);
            vm2.ResolveEditDeleteRights("", targetEntity);

            Assert.True(vm.EditEnabled);
            Assert.True(vm.DeleteEnabled);

            Assert.False(vm2.EditEnabled);
            Assert.False(vm2.DeleteEnabled);
        }

        [Fact]
        public void Resolve_CreateDownload_Rights_Updates_CreateDownload_Rights()
        {
            TestItemsBaseVM vm = new();
            TestItemsBaseVM vm2 = new();

            String targetEntity = "TestEntity";
            String userRoleClaims = $"{targetEntity}:create,{targetEntity}:download";

            vm.ResolveCreateDownloadRights(userRoleClaims, targetEntity);
            vm2.ResolveCreateDownloadRights("", targetEntity);

            Assert.True(vm.CreateEnabled);
            Assert.True(vm.DownloadEnabled);

            Assert.False(vm2.CreateEnabled);
            Assert.False(vm2.DownloadEnabled);
        }



    }
}

