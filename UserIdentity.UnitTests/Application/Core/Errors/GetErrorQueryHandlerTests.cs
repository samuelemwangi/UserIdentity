using System;
using System.Threading.Tasks;

using FakeItEasy;

using UserIdentity.Application.Core.Errors.Queries.GerError;
using UserIdentity.Application.Enums;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Infrastructure.Utilities;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Errors
{
	public class GetErrorQueryHandlerTests
	{
		private readonly IMachineDateTime _machineDateTime;
		private readonly IStringHelper _stringHelper;
		private readonly ILogHelper<GetErrorQueryHandler> _logHelper;


		public GetErrorQueryHandlerTests()
		{
			_machineDateTime = new MachineDateTime();
			_stringHelper = new StringHelper();
			_logHelper = A.Fake<ILogHelper<GetErrorQueryHandler>>();
		}

		[Fact]
		public async Task Get_Error_Returns_Error_ViewModel()
		{
			// Arrange
			var errorMessage = "Testing error message";
			var statusMessage = "Failed badly";


			var query = new GetErrorQuery()
			{
				Exception = new Exception(errorMessage),
				ErrorMessage = errorMessage,
				StatusMessage = statusMessage
			};


			var getErrorQueryHandler = new GetErrorQueryHandler(_machineDateTime, _stringHelper, _logHelper);

			// Act
			var vm = await getErrorQueryHandler.GetItemAsync(query);


			// Assert
			Assert.NotNull(vm);

			Assert.Equal(statusMessage.ToUpper(), vm.StatusMessage);
			Assert.Equal(RequestStatus.FAILED.GetDisplayName(), vm.RequestStatus);

			Assert.NotNull(vm.Error);

			Assert.IsType<string>(vm.Error?.Message);
			Assert.NotNull(vm.Error?.Message);

			Assert.IsType<DateTime>(vm.Error?.Timestamp);
			Assert.NotNull(vm.Error?.Timestamp);
		}

		[Fact]
		public async Task Get_Error_With_Null_Status_Message_Returns_Error_ViewMode()
		{
			// Arrange
			var query = new GetErrorQuery()
			{
				Exception = new Exception("Testing error message"),
				ErrorMessage = "Testing error message",
				StatusMessage = null
			};

			var getErrorQueryHandler = new GetErrorQueryHandler(_machineDateTime, _stringHelper, _logHelper);

			// Act
			var vm = await getErrorQueryHandler.GetItemAsync(query);

			// Assert
			Assert.NotNull(vm);

			Assert.Equal(ItemStatusMessage.FETCH_ITEM_FAILED.GetDisplayName(), vm.StatusMessage);
			Assert.Equal(RequestStatus.FAILED.GetDisplayName(), vm.RequestStatus);

			Assert.NotNull(vm.Error);

			Assert.IsType<string>(vm.Error?.Message);
			Assert.NotNull(vm.Error?.Message);

			Assert.IsType<DateTime>(vm.Error?.Timestamp);
			Assert.NotNull(vm.Error?.Timestamp);
		}
	}

}

