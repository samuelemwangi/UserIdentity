using System;
using UserIdentity.Application.Exceptions;
using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
	public class RecordExistsExceptionTest
	{
        [Fact]
        public async void Illegal_Event_With_Message_Throws_RecordExistsException()
        {
            String message = "Record already exists";

            var exception = await Assert.ThrowsAsync<RecordExistsException>(() => throw new RecordExistsException(message));

            Assert.Equal(exception.Message, message);
        }

        [Fact]
        public async void Illegal_Event_With_Message_and_Classname_Throws_RecordExistsException()
        {
            String className = "RecordExistsClass";
            String id = "123QWERTY";

            String expectedMessage = className + ": A record identified with - " + id + " - exists";

            var exception = await Assert.ThrowsAsync<RecordExistsException>(() => throw new RecordExistsException(id, className));

            Assert.Equal(exception.Message, expectedMessage);
        }
    }
}

