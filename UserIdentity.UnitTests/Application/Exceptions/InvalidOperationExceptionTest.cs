using UserIdentity.Application.Exceptions;
using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
    public class InvalidOperationExceptionTest
    {

        [Fact]
        public async void Illegal_Event_With_Message_Throws_InvalidOperationException()
        {
            string message = "An invalid operation occured";

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => throw new InvalidOperationException(message));

            Assert.Equal(exception.Message, message);
        }

        [Fact]
        public async void Illegal_Event_With_Message_and_Classname_Throws_InvalidOperationException()
        {
            string message = "An invalid operation occured";
            string className = "InvalidOperationTester";

            string expectedMessage = className + ": The operation - " + message + " - is not allowed";

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => throw new InvalidOperationException(message, className));

            Assert.Equal(exception.Message, expectedMessage);
        }
    }
}

