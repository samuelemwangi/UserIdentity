using System.Threading.Tasks;

using UserIdentity.Application.Exceptions;

using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
  public class RecordUpdateExceptionTests
  {

    [Fact]
    public async Task Record_Update_Error_With_Message_Throws_RecordUpdateException()
    {
      // Arrange
      var message = "Record update failed";

      // Act & Assert
      var exception = await Assert.ThrowsAsync<RecordUpdateException>(() => throw new RecordUpdateException(message));

      Assert.Equal(exception.Message, message);
    }

    [Fact]
    public async Task Record_Update_Error_With_Message_and_Classname_Throws_RecordUpdateException()
    {
      // Arrange
      var className = "RecordExistsClass";
      var id = "123QWERTY";

      var expectedMessage = className + ": An error occured while updating a record identified by - " + id;

      // Act & Assert
      var exception = await Assert.ThrowsAsync<RecordUpdateException>(() => throw new RecordUpdateException(id, className));

      Assert.Equal(exception.Message, expectedMessage);
    }
  }
}

