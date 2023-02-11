using System;
using System.Threading.Tasks;

using FakeItEasy;

using UserIdentity.Application.Core.Errors.Queries.GerError;
using UserIdentity.Application.Core.Errors.ViewModels;
using UserIdentity.Application.Enums;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Infrastructure.Utilities;

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
			_machineDateTime = new MachineDateTime();
			_stringHelper = new StringHelper();
			_logHelper = A.Fake<ILogHelper<GetErrorQueryHandler>>();
		}

		[Fact]
		public async Task Get_Error_Returns_ErrorViewModel()
		{
			// Arrange
			var errorMessage = "Testing error message";
			var statusMessage = "Failed badly";


			GetErrorQuery query = new()
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

			Assert.Equal(vm.StatusMessage, statusMessage.ToUpper());
			Assert.Equal(vm.RequestStatus, RequestStatus.FAILED.GetDisplayName());

			Assert.NotNull(vm.Error);

			Assert.IsType<String>(vm.Error?.Message);
			Assert.NotNull(vm.Error?.Message);

			Assert.IsType<DateTime>(vm.Error?.Timestamp);
			Assert.NotNull(vm.Error?.Timestamp);

		}
	}

}

