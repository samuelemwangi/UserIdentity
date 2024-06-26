﻿using System;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

using UserIdentity.Application.Core.Errors.Queries.GerError;
using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Presentation.Helpers;
using UserIdentity.UnitTests.TestUtils;

using Xunit;

using static System.Net.Mime.MediaTypeNames;

using InvalidOperationException = UserIdentity.Application.Exceptions.InvalidOperationException;

namespace UserIdentity.UnitTests.Presentation.Helpers
{
	public class ExceptionMiddlewareTests
	{
		[Fact]
		public async Task Invoke_Exception_Middleware_Invokes_Exception_Middleware()
		{
			// Arrange
			var responseBodyString = TestStringHelper.GenerateRandomString(100);

			RequestDelegate next = (HttpContext context) =>
			{
				context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
				context.Response.WriteAsync(responseBodyString);
				return Task.CompletedTask;
			};


			HttpContext context = new DefaultHttpContext();
			context.Response.Body = new MemoryStream();

			var middleware = new ExceptionMiddleware(next);

			var getErrorQueryHandler = A.Fake<GetErrorQueryHandler>();
			var logHelper = A.Fake<ILogHelper<ExceptionMiddleware>>();

			// Act
			await middleware.InvokeAsync(context, getErrorQueryHandler, logHelper);


			//Assert
			Assert.Equal(HttpStatusCode.UnprocessableEntity, (HttpStatusCode)context.Response.StatusCode);

			context.Response.Body.Seek(0, SeekOrigin.Begin);
			var body = new StreamReader(context.Response.Body).ReadToEnd();

			Assert.Equal(responseBodyString, body);
		}

		[Theory]
		[InlineData(typeof(NoRecordException), HttpStatusCode.NotFound)]
		[InlineData(typeof(RecordExistsException), HttpStatusCode.BadRequest)]

		[InlineData(typeof(RecordCreationException), HttpStatusCode.InternalServerError)]
		[InlineData(typeof(RecordUpdateException), HttpStatusCode.InternalServerError)]

		[InlineData(typeof(InvalidOperationException), HttpStatusCode.NotAcceptable)]

		[InlineData(typeof(SecurityTokenException), HttpStatusCode.Unauthorized)]
		[InlineData(typeof(SecurityTokenExpiredException), HttpStatusCode.Unauthorized, "An expired access token was provided")]
		[InlineData(typeof(SecurityTokenReadException), HttpStatusCode.Unauthorized, "An invalid access token was provided")]

		[InlineData(typeof(InvalidCredentialException), HttpStatusCode.Unauthorized, "Provided credentials are invalid")]

		[InlineData(typeof(MissingConfigurationException), HttpStatusCode.InternalServerError, "An application error occured")]

		[InlineData(typeof(DivideByZeroException), HttpStatusCode.InternalServerError, "An internal application error occured")]
		[InlineData(typeof(Exception), HttpStatusCode.InternalServerError, "An internal application error occured")]
		public async Task Invoke_Exception_Middleware_With_Exception_Returns_Error_Response(Type exceptionType, HttpStatusCode httpStatusCode, string errorMessage = "")
		{
			// Arrange
			var exceptionErrorMessage = "Error" + TestStringHelper.GenerateRandomString(5);
			var expectedExceptionErrorMessage = errorMessage == "" ? exceptionErrorMessage : errorMessage;

			RequestDelegate next = (HttpContext context) =>
			{
				ThrowException(exceptionType, exceptionErrorMessage);
				return Task.CompletedTask;
			};


			HttpContext context = new DefaultHttpContext();
			context.Response.Body = new MemoryStream();

			var middleware = new ExceptionMiddleware(next);

			var getErrorQueryHandler = A.Fake<GetErrorQueryHandler>();
			var logHelper = A.Fake<ILogHelper<ExceptionMiddleware>>();

			// Act
			await middleware.InvokeAsync(context, getErrorQueryHandler, logHelper);


			// Assert
			Assert.Equal(httpStatusCode, (HttpStatusCode)context.Response.StatusCode);

			context.Response.Body.Seek(0, SeekOrigin.Begin);
			var body = new StreamReader(context.Response.Body).ReadToEnd();

			Assert.Contains(expectedExceptionErrorMessage, body);
		}

		private static void ThrowException(Type exceptionType, string exceptionErrorMessage)
		{
			Exception exception = (Exception)exceptionType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { exceptionErrorMessage });
			throw exception;
		}
	}
}
