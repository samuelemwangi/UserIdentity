﻿using Microsoft.AspNetCore.Mvc.ModelBinding;

using Newtonsoft.Json;

using System.Net;

using UserIdentity.Application.Enums;

namespace UserIdentity.Presentation.Helpers.ValidationExceptions
{
  public class ValidationError
  {
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public String Field { get; }

    public String Message { get; }

    public ValidationError(String field, String message)
    {
      Field = field;
      Message = message;
    }
  }

  public class ValidationErrorDTO
  {
    public String? Message { get; internal set; }
    public DateTime? Timestamp { get; internal set; }
    public List<ValidationError>? ErrorList { get; internal set; }
  }

  public class ValidationResultModel
  {
    public String? RequestStatus { get; internal set; }
    public String? StatusMessage { get; internal set; }

    public ValidationErrorDTO? Error { get; internal set; }


    public ValidationResultModel(ModelStateDictionary modelState)
    {
      HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;

      RequestStatus = Application.Enums.RequestStatus.FAILED.GetDisplayName();
      StatusMessage = (Int32)httpStatusCode + " - BAD REQUEST";

      var errorList = new List<ValidationError>();

      foreach (var item in modelState)
      {
        if (item.Value.Errors.Count > 0)
        {
          errorList.AddRange(item.Value.Errors.Select(x => new ValidationError(item.Key, x.ErrorMessage)).ToList());
        }

      }
      Error = new ValidationErrorDTO
      {
        Message = "Validation Failed",
        Timestamp = DateTime.UtcNow,
        ErrorList = errorList
      };
    }
  }
}
