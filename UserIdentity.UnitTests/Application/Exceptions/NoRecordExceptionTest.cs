using System;
using UserIdentity.Application.Exceptions;
using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
	public class NoRecordExceptionTest
	{
        [Fact]
        public async void Illegal_Event_With_Message_Throws_NoRecordException()
        {
            String message = "No record found";

            var exception = await Assert.ThrowsAsync<NoRecordException>(() => throw new NoRecordException(message));

            Assert.Equal(exception.Message, message);
        }

        [Fact]
        public async void Illegal_Event_With_Message_and_Classname_Throws_NoRecordException()
        {
            String className = "NoRecordClass";
            String id = "123QWERTY";

            String expectedMessage = className + ": No record exists for the provided identifier - " + id;

            var exception = await Assert.ThrowsAsync<NoRecordException>(() => throw new NoRecordException(id, className));

            Assert.Equal(exception.Message, expectedMessage);
        }
    }
}

