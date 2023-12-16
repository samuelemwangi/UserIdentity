using System.Threading.Tasks;

using UserIdentity.Application.Exceptions;

using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
	public class RecordExistsExceptionTests
	{
		[Fact]
		public async Task Record_Exists_Error_With_Message_Throws_RecordExistsException()
		{
			// Arrange
			var message = "Record already exists";

			// Act & Assert
			var exception = await Assert.ThrowsAsync<RecordExistsException>(() => throw new RecordExistsException(message));

			Assert.Equal(exception.Message, message);
		}

		[Fact]
		public async Task Record_Exists_Error_With_Message_and_Classname_Throws_RecordExistsException()
		{
			// Arrnge
			var className = "RecordExistsClass";
			var id = "123QWERTY";

			var expectedMessage = className + ": A record identified with - " + id + " - exists";


			// Act & Assert
			var exception = await Assert.ThrowsAsync<RecordExistsException>(() => throw new RecordExistsException(id, className));

			Assert.Equal(exception.Message, expectedMessage);
		}
	}
}

