using Microsoft.Extensions.Primitives;

using UserIdentity.Application.Core.Errors.Queries.GerError;
using UserIdentity.Application.Core.Errors.ViewModels;
using UserIdentity.Application.Core.Interfaces;

namespace UserIdentity.Presentation.Helpers
{
	public class RequestIdMiddleware
	{
		private readonly RequestDelegate _next;

		public RequestIdMiddleware(RequestDelegate requestDelegate)
		{
			_next = requestDelegate;
		}

		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				string requestId = Guid.NewGuid().ToString();
				if (httpContext.Request.Headers.TryGetValue("X-REQUEST-ID", out StringValues headerRequestId))
					requestId = headerRequestId.ToString();

				httpContext.Request.Headers["X-Request-Id"] = requestId;
				httpContext.Response.Headers["X-Request-Id"] = requestId;

				await _next(httpContext);
			}
			catch (Exception)
			{
				throw;
			}

		}
	}
}
