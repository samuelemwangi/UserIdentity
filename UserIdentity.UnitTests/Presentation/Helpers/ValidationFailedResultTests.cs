using System;
using System.Net;

using Microsoft.AspNetCore.Mvc.ModelBinding;

using UserIdentity.Presentation.Helpers.ValidationExceptions;

using Xunit;

namespace UserIdentity.UnitTests.Presentation.Helpers
{
	public class ValidationFailedResultTests
	{

		[Fact]
		public void New_ValidationFailedResult_Creates_New_ValidationFailedResult_Instance()
		{
			// Arrange
			var modelStateDictionary = new ModelStateDictionary();

			// Act
			var validationFailedResult = new ValidationFailedResult(modelStateDictionary);

			// Assert
			Assert.NotNull(validationFailedResult);
			Assert.Equal((Int32)HttpStatusCode.BadRequest, validationFailedResult.StatusCode);
		}
	}
}
