using System.Net;
using System.Security.Authentication;
using System.Text.Json;

using Microsoft.IdentityModel.Tokens;

using UserIdentity.Application.Core.Errors.Queries.GerError;
using UserIdentity.Application.Core.Errors.ViewModels;
using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Exceptions;

namespace UserIdentity.Presentation.Helpers
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		public ExceptionMiddleware(RequestDelegate requestDelegate)
		{
			_next = requestDelegate;

		}

		public async Task InvokeAsync(HttpContext httpContext, IGetItemQueryHandler<GetErrorQuery, ErrorViewModel> getErrorQueryHandler)
		{

			IGetItemQueryHandler<GetErrorQuery, ErrorViewModel> _getErrorQueryHandler = getErrorQueryHandler;

			try
			{
				await _next(httpContext);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(httpContext, ex, _getErrorQueryHandler);
			}

		}

		private static async Task HandleExceptionAsync(HttpContext context, Exception exception, IGetItemQueryHandler<GetErrorQuery, ErrorViewModel> _getErrorQueryHandler)
		{
			context.Response.ContentType = "application/json";
			HttpStatusCode statusCode;
			String errorMessage;
			if (typeof(NoRecordException).IsInstanceOfType(exception))
			{
				errorMessage = exception.Message;
				statusCode = HttpStatusCode.NotFound;
			}
			else if (typeof(RecordExistsException).IsInstanceOfType(exception))
			{
				errorMessage = exception.Message;
				statusCode = HttpStatusCode.BadRequest;
			}
			else if (typeof(RecordCreationException).IsInstanceOfType(exception) || typeof(RecordUpdateException).IsInstanceOfType(exception))
			{
				errorMessage = exception.Message;
				statusCode = HttpStatusCode.InternalServerError;
			}
			else if (typeof(Application.Exceptions.InvalidOperationException).IsInstanceOfType(exception))
			{
				errorMessage = exception.Message;
				statusCode = HttpStatusCode.NotAcceptable;
			}
			else if (typeof(SecurityTokenExpiredException).IsInstanceOfType(exception))
			{
				errorMessage = "An expired access token was provided";
				statusCode = HttpStatusCode.Unauthorized;
			}
			else if (typeof(SecurityTokenException).IsInstanceOfType(exception))
			{
				errorMessage = exception.Message;
				statusCode = HttpStatusCode.Unauthorized;
			}
			else if (typeof(SecurityTokenReadException).IsInstanceOfType(exception))
			{
				errorMessage = "An invalid access token was provided";
				statusCode = HttpStatusCode.Unauthorized;
			}
			else if (typeof(InvalidCredentialException).IsInstanceOfType(exception))
			{
				errorMessage = "Provided credentials are invalid";
				statusCode = HttpStatusCode.Unauthorized;
			}
			else
			{
				statusCode = HttpStatusCode.InternalServerError;
				errorMessage = exception.Message;
			}

			var errorViewModel = await _getErrorQueryHandler.GetItemAsync(new GetErrorQuery
			{
				Exception = exception,
				ErrorMessage = errorMessage,
				StatusMessage = (int)statusCode + " -" + statusCode.ToString()
			});

			context.Response.StatusCode = (int)statusCode;

			if (errorViewModel.Error != null)
			{
				errorViewModel.Error.Message = errorMessage.Substring(errorMessage.IndexOf(":") + 1).Trim();
			}

			var serializerOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};


			await context.Response.WriteAsync(JsonSerializer.Serialize(errorViewModel, serializerOptions));
		}
	}
}
