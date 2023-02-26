using System.Threading.Tasks;

using UserIdentity.Application.Exceptions;

using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
	public class SecurityTokenReadExceptionTests
	{
		[Fact]
		public async Task Security_Token_Read_Error_With_Message_Throws_SecurityTokenReadException()
		{
			// Arrange
			var message = "Security TokenRead Exception () *";

			// Act & Assert
			var exception = await Assert.ThrowsAsync<SecurityTokenReadException>(() => throw new SecurityTokenReadException(message));

			Assert.Equal(exception.Message, message);
		}
	}
}

