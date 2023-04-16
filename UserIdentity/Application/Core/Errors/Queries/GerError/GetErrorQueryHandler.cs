using UserIdentity.Application.Core.Errors.ViewModels;
using UserIdentity.Application.Core.Extensions;
using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Enums;
using UserIdentity.Application.Interfaces.Utilities;

namespace UserIdentity.Application.Core.Errors.Queries.GerError
{
    public record GetErrorQuery
	{
		public Exception Exception { get; internal set; }
		public String? ErrorMessage { get; internal set; }
		public String? StatusMessage { get; internal set; }
	}

	public class GetErrorQueryHandler : IGetItemQueryHandler<GetErrorQuery, ErrorViewModel>
	{
		private readonly IMachineDateTime _machineDateTime;
		private readonly IStringHelper _stringHelper;
		private readonly ILogHelper<GetErrorQueryHandler> _logHelper;

		public GetErrorQueryHandler(IMachineDateTime machineDateTime, IStringHelper stringHelper, ILogHelper<GetErrorQueryHandler> logHelper)
		{
			_machineDateTime = machineDateTime;
			_stringHelper = stringHelper;
			_logHelper = logHelper;
		}

		public async Task<ErrorViewModel> GetItemAsync(GetErrorQuery query)
		{
			// log the error 
			_logHelper.LogEvent(query.Exception.Message, LogLevel.Error);

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
