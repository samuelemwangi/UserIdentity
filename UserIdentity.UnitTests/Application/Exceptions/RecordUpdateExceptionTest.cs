using System;
using UserIdentity.Application.Exceptions;
using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
	public class RecordUpdateExceptionTest
	{

        [Fact]
        public async void Illegal_Event_With_Message_Throws_RecordUpdateException()
        {
            String message = "Record update failed";

            var exception = await Assert.ThrowsAsync<RecordUpdateException>(() => throw new RecordUpdateException(message));

            Assert.Equal(exception.Message, message);
        }

        [Fact]
        public async void Illegal_Event_With_Message_and_Classname_Throws_RecordUpdateException()
        {
            String className = "RecordExistsClass";
            String id = "123QWERTY";

            String expectedMessage = className + ": An error occured while updating a record identified by - " + id;

            var exception = await Assert.ThrowsAsync<RecordUpdateException>(() => throw new RecordUpdateException(id, className));

            Assert.Equal(exception.Message, expectedMessage);
        }
    }
}

