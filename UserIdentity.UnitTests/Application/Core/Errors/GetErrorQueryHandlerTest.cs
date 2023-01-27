using System;
using FakeItEasy;
using UserIdentity.Application.Core.Errors.Queries.GerError;
using UserIdentity.Application.Core.Errors.ViewModels;
using UserIdentity.Application.Enums;
using UserIdentity.Application.Interfaces.Utilities;
using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Errors
{
    public class GetErrorQueryHandlerTest
    {
        private readonly IMachineDateTime _machineDateTime;
        private readonly IStringHelper _stringHelper;
        private readonly ILogHelper<GetErrorQueryHandler> _logHelper;


        public GetErrorQueryHandlerTest()
        {
            _machineDateTime = A.Fake<IMachineDateTime>();
            _stringHelper = A.Fake<IStringHelper>();
            _logHelper = A.Fake<ILogHelper<GetErrorQueryHandler>>();
        }

        [Fact]
        public void GetError_Returns_ErrorViewModel()
        {
            String errorMessage = "Testing error message";
            String statusMessage = "Failed badly";

            GetErrorQuery query = new GetErrorQuery
            {
                Exception = new Exception(errorMessage),
                ErrorMessage = errorMessage,
                StatusMessage = statusMessage
            };


            GetErrorQueryHandler getErrorQueryHandler = new GetErrorQueryHandler(_machineDateTime, _stringHelper, _logHelper);
            var errorVM =  getErrorQueryHandler.GetError(query);

            var result = errorVM.Result as ErrorViewModel;

            Assert.NotNull(result);

            Assert.Equal(result.StatusMessage, statusMessage);
            Assert.Equal(result.RequestStatus, RequestStatus.FAILED.GetDisplayName());

            Assert.NotNull(result.Error);

            Assert.IsType<String>(result.Error?.Message);
            Assert.NotNull(result.Error?.Message);

            Assert.IsType<DateTime>(result.Error?.Timestamp);
            Assert.NotNull(result.Error?.Timestamp);

        }
    }

}

