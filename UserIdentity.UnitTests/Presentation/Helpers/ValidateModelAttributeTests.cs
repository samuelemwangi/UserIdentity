using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Net;
using UserIdentity.Presentation.Helpers.ValidationExceptions;
using Xunit;

namespace UserIdentity.UnitTests.Presentation.Helpers
{
  public class ValidateModelAttributeTests
  {

    [Fact]
    public void ValidateModelAttribute_Validate_Model_Attribute()
    {
      // Arrange
      var validateModelAttribute = new ValidateModelAttribute();
      var httpContext = new DefaultHttpContext();
      var modelStateDictionary = new ModelStateDictionary();

      modelStateDictionary.AddModelError("Test", "Test");
      var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor(), modelStateDictionary);
      var actionExecutingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<String, object?>(), controller: new object());


      // Act
      validateModelAttribute.OnActionExecuting(actionExecutingContext);

      // Assert
      Assert.NotNull(actionExecutingContext.Result);
      Assert.Equal((Int32)HttpStatusCode.BadRequest, (actionExecutingContext.Result as ValidationFailedResult)?.StatusCode);
    }
  }
}
