using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Net;
using UserIdentity.Presentation.Helpers.ValidationExceptions;
using Xunit;

namespace UserIdentity.UnitTests.Presentation.Helpers
{
  public class ValidationResultModelTests
  {

    [Fact]
    public void New_ValidationResultModel_Creates_New_ValidationResultModel_Instance()
    {
      // Arrange & Act
      var errorKey = Guid.NewGuid().ToString();
      var errorValue = Guid.NewGuid().ToString() + " Error Test Value";
      var modelStateDictionary = new ModelStateDictionary();
      modelStateDictionary.AddModelError(errorKey, errorValue);

      var validationResultModel = new ValidationResultModel(modelStateDictionary);

      // Assert
      Assert.NotNull(validationResultModel);
      Assert.Contains((Int32)HttpStatusCode.BadRequest + "", validationResultModel.StatusMessage);
      Assert.NotNull(validationResultModel.Error);
      Assert.Equal(1, validationResultModel.Error?.ErrorList?.Count);

      Assert.True(validationResultModel.Error?.ErrorList?.Where(e => e.Field == errorKey && e.Message == errorValue).Any());
    }

  }
}
