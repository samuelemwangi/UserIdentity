using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Net;
using UserIdentity.Application.Enums;

namespace UserIdentity.Presentation.Helpers.ValidationExceptions
{
	public class ValidationError
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public String? Field { get; }

		public String? Message { get; }

		public ValidationError(String? field, String? message)
		{
			Field = field != String.Empty ? field : null;
			Message = message;
		}
	}

	public class ErrorDTO
	{
		public String? Message { get; internal set; }
		public DateTime? Timestamp { get; internal set; }
		public List<ValidationError>? ErrorList { get; internal set; }
	}

	public class ValidationResultModel
	{
		public String? RequestStatus { get; internal set; }
		public String? StatusMessage { get; internal set; }

		public ErrorDTO? Error { get; internal set; }


		public ValidationResultModel(ModelStateDictionary modelState)
		{
			HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;

			RequestStatus = Application.Enums.RequestStatus.FAILED.GetDisplayName();
			StatusMessage = (int)httpStatusCode + " - BAD REQUEST";
#pragma warning disable CS8602 // Dereference of a possibly null reference.
			Error = new ErrorDTO
			{
				Message = "Validation Failed",
				Timestamp = DateTime.UtcNow,
				ErrorList = modelState.Keys
							.SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
							.ToList()
			};
#pragma warning restore CS8602 // Dereference of a possibly null reference.

		}
	}
}
