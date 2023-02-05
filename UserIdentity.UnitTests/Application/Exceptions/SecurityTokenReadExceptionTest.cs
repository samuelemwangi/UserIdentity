using System;
using UserIdentity.Application.Exceptions;
using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
	public class SecurityTokenReadExceptionTest
	{
        [Fact]
        public async void Illegal_Event_With_Message_Throws_SecurityTokenReadException()
        {
            String message = "Security TokenRead Exception () *";

            var exception = await Assert.ThrowsAsync<SecurityTokenReadException>(() => throw new SecurityTokenReadException(message));

            Assert.Equal(exception.Message, message);
        }
    }
}

