using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UserIdentity.Presentation.Helpers.ValidationExceptions
{
	public class ValidationFailedResult : ObjectResult
	{
		public ValidationFailedResult(ModelStateDictionary modelState)
				: base(new ValidationResultModel(modelState))
		{
			StatusCode = StatusCodes.Status422UnprocessableEntity;
		}
	}
}
