using System;

using UserIdentity.Application.Exceptions;

using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
	public class IllegalEventExceptionTest
	{
		[Fact]
		public async void Illegal_Event_With_Message_Throws_IllegalEventException()
		{
			// Arrange
			var message = "An illegal event occured";

			// Act & Assert
			var exception = await Assert.ThrowsAsync<IllegalEventException>(() => throw new IllegalEventException(message));

			Assert.Equal(exception.Message, message);
		}

		[Fact]
		public async void Illegal_Event_With_Message_and_Classname_Throws_IllegalEventException()
		{
			// Arrange
			var message = "An illegal event occured";
			var className = "IllegalEventTester";

			var expectedMessage = className + ": The event - " + message + " - is not allowed";

			// Act
			var exception = await Assert.ThrowsAsync<IllegalEventException>(() => throw new IllegalEventException(message, className));

			// Assert
			Assert.Equal(exception.Message, expectedMessage);
		}

	}
}

