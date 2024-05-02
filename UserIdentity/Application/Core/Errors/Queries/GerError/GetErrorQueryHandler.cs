using UserIdentity.Application.Core.Errors.ViewModels;
using UserIdentity.Application.Core.Extensions;
using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Enums;
using UserIdentity.Application.Interfaces.Utilities;

namespace UserIdentity.Application.Core.Errors.Queries.GerError
{
	public record GetErrorQuery
	{
		public string? ErrorMessage { get; internal set; }
		public string? StatusMessage { get; internal set; }
	}

	public class GetErrorQueryHandler : IGetItemQueryHandler<GetErrorQuery, ErrorViewModel>
	{
		private readonly IMachineDateTime _machineDateTime;
		private readonly IStringHelper _stringHelper;

		public GetErrorQueryHandler(IMachineDateTime machineDateTime, IStringHelper stringHelper)
		{
			_machineDateTime = machineDateTime;
			_stringHelper = stringHelper;
		}

		public async Task<ErrorViewModel> GetItemAsync(GetErrorQuery query)
		{
			
			var errorDTO = new ErrorDTO
			{
				Timestamp = _machineDateTime.Now,
				Message = query.ErrorMessage
			};

			var errorVM = new ErrorViewModel
			{
				Error = errorDTO,
			};

			var statusMessage = query.StatusMessage != null ? _stringHelper.AddSpacesToSentence(query.StatusMessage, true).ToUpper() : "";


			errorVM.ResolveRequestStatus(RequestStatus.FAILED, ItemStatusMessage.FETCH_ITEM_FAILED, statusMessage);

			return await Task.FromResult(errorVM);

		}
	}
}
