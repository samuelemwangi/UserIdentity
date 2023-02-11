using System;

using UserIdentity.Application.Exceptions;

using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
	public class RecordDeletionExceptionTest
	{
		[Fact]
		public async void Record_Deletion_Error_With_Message_Throws_RecordDeletionException()
		{
			// Arrange
			var message = "Record deletion failed";


			// Act & Assert
			var exception = await Assert.ThrowsAsync<RecordDeletionException>(() => throw new RecordDeletionException(message));

			Assert.Equal(exception.Message, message);
		}

		[Fact]
		public async void Record_Deletion_Error_With_Message_and_Classname_Throws_RecordDeletionException()
		{
			// Arrange
			var className = "RecordDeletionClass";
			var id = "123QWERTY";

			var expectedMessage = className + ": An error occured while deleting a record identified by - " + id;


			// Act & Assert
			var exception = await Assert.ThrowsAsync<RecordDeletionException>(() => throw new RecordDeletionException(id, className));

			Assert.Equal(exception.Message, expectedMessage);
		}
	}
}

