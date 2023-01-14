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

    public class ViewModelExtensionsTest
    {
        [Fact]
        public void Resolve_Request_Status_Updates_VM_Status()
        {
            TestBaseVM vm = new TestBaseVM();

            vm.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

            Assert.Equal(RequestStatus.SUCCESSFUL.GetDisplayName(), vm.RequestStatus);
            Assert.Equal(ItemStatusMessage.SUCCESS.GetDisplayName(), vm.StatusMessage);
        }

        [Fact]
        public void Resolve_StatusMessage_Status_Updates_VM_StatusMessage()
        {
            TestBaseVM vm = new TestBaseVM();
            String customMesage = "Failed but looks like success";

            vm.ResolveRequestStatus(RequestStatus.FAILED, ItemStatusMessage.SUCCESS, customMesage);

            Assert.Equal(RequestStatus.FAILED.GetDisplayName(), vm.RequestStatus);
            Assert.Equal(customMesage, vm.StatusMessage);
        }

    }
}

