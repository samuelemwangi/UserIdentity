using System;
using UserIdentity.Application.Exceptions;
using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
	public class RecordCreationExceptionTest
	{
        [Fact]
        public async void Illegal_Event_With_Message_Throws_RecordCreationException()
        {
            String message = "Record creation failed";

            var exception = await Assert.ThrowsAsync<RecordCreationException>(() => throw new RecordCreationException(message));

            Assert.Equal(exception.Message, message);
        }

        [Fact]
        public async void Illegal_Event_With_Message_and_Classname_Throws_RecordCreationException()
        {
            String className = "RecordCreationClass";
            String id = "123QWERTY";

            String expectedMessage = className + ": An error occured while creating a record identified by - " + id;

            var exception = await Assert.ThrowsAsync<RecordCreationException>(() => throw new RecordCreationException(id, className));

            Assert.Equal(exception.Message, expectedMessage);
        }
    }
}

