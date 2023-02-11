using System;

using UserIdentity.Application.Exceptions;

using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
	public class NoRecordExceptionTest
	{
		[Fact]
		public async void No_Record_Error_With_Message_Throws_NoRecordException()
		{
			// Arrange
			var message = "No record found";

			// Act & Assert
			var exception = await Assert.ThrowsAsync<NoRecordException>(() => throw new NoRecordException(message));

			Assert.Equal(exception.Message, message);
		}

		[Fact]
		public async void No_Record_Error_With_Message_and_Classname_Throws_NoRecordException()
		{
			// Arrange
			var className = "NoRecordClass";
			var id = "123QWERTY";

			var expectedMessage = className + ": No record exists for the provided identifier - " + id;


			// Act & Assert
			var exception = await Assert.ThrowsAsync<NoRecordException>(() => throw new NoRecordException(id, className));

			Assert.Equal(exception.Message, expectedMessage);
		}
	}
}

