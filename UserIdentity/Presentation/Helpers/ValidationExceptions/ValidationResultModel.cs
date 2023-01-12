using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Net;
using UserIdentity.Application.Enums;

namespace UserIdentity.Presentation.Helpers.ValidationExceptions
{
	public class ValidationError
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Field { get; }

		public string? Message { get; }

		public ValidationError(string? field, string? message)
		{
			Field = field != string.Empty ? field : null;
			Message = message;
		}
	}

	public class ErrorDTO
	{
		public string? Message { get; internal set; }
		public DateTime? Timestamp { get; internal set; }
		public List<ValidationError>? ErrorList { get; internal set; }
	}

	public class ValidationResultModel
	{
		public string? RequestStatus { get; internal set; }
		public string? StatusMessage { get; internal set; }

		public ErrorDTO? Error { get; internal set; }


		public ValidationResultModel(ModelStateDictionary modelState)
		{
			HttpStatusCode httpStatusCode = HttpStatusCode.UnprocessableEntity;

			RequestStatus = Application.Enums.RequestStatus.FAILED.GetDisplayName();
			StatusMessage = (int)httpStatusCode + " - UNPROCESSABLE ENTITY";
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
