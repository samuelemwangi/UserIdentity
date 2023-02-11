using UserIdentity.Application.Exceptions;

using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
	public class InvalidOperationExceptionTest
	{

		[Fact]
		public async void Invalid_Operation_With_Message_Throws_InvalidOperationException()
		{
			// Arrange
			var message = "An invalid operation occured";

			// Act  & Assert
			var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => throw new InvalidOperationException(message));

			Assert.Equal(exception.Message, message);
		}

		[Fact]
		public async void Invalid_Operation_With_Message_and_Classname_Throws_InvalidOperationException()
		{
			// Arrange
			var message = "An invalid operation occured";
			var className = "InvalidOperationTester";

			var expectedMessage = className + ": The operation - " + message + " - is not allowed";

			// Act & Assert
			var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => throw new InvalidOperationException(message, className));

			Assert.Equal(exception.Message, expectedMessage);
		}
	}
}

