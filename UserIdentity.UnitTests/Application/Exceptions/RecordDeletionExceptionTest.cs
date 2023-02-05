using System;
using UserIdentity.Application.Exceptions;
using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
    public class RecordDeletionExceptionTest
    {
        [Fact]
        public async void Illegal_Event_With_Message_Throws_RecordDeletionException()
        {
            String message = "Record deletion failed";

            var exception = await Assert.ThrowsAsync<RecordDeletionException>(() => throw new RecordDeletionException(message));

            Assert.Equal(exception.Message, message);
        }

        [Fact]
        public async void Illegal_Event_With_Message_and_Classname_Throws_RecordDeletionException()
        {
            String className = "RecordDeletionClass";
            String id = "123QWERTY";

            String expectedMessage = className + ": An error occured while deleting a record identified by - " + id;

            var exception = await Assert.ThrowsAsync<RecordDeletionException>(() => throw new RecordDeletionException(id, className));

            Assert.Equal(exception.Message, expectedMessage);
        }
    }
}

