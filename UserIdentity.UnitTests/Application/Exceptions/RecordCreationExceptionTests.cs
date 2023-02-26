using System.Threading.Tasks;

using UserIdentity.Application.Exceptions;

using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
	public class RecordCreationExceptionTests
	{
		[Fact]
		public async Task Record_Creation_Error_With_Message_Throws_RecordCreationException()
		{
			// Arrange
			var message = "Record creation failed";

			// Act & Assert
			var exception = await Assert.ThrowsAsync<RecordCreationException>(() => throw new RecordCreationException(message));

			Assert.Equal(exception.Message, message);
		}

		[Fact]
		public async Task Record_Creation_Error_With_Message_and_Classname_Throws_RecordCreationException()
		{
			// Arrange
			var className = "RecordCreationClass";
			var id = "123QWERTY";
			var expectedMessage = className + ": An error occured while creating a record identified by - " + id;


			// Act & Assert
			var exception = await Assert.ThrowsAsync<RecordCreationException>(() => throw new RecordCreationException(id, className));

			Assert.Equal(exception.Message, expectedMessage);
		}
	}
}

