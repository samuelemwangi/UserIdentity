using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using UserIdentity.Presentation.Controllers;

namespace UserIdentity.UnitTests.Presentation.Controllers
{
	internal static class ControllerUtils
	{
		public static string UserId = "1234567890";
		public static string UserRoles = "role1,role2,role3";
		public static string UserScopes = "scope1:edit,scope2:read,scope3:delete";

		public static void UpdateContext(this BaseController controller, string? controllerName, bool addUserId = false, bool addUserRoles = false, bool addUserScopes = false)
		{
			// Route data
			var routedData = new RouteData();
			routedData.Values["controller"] = controllerName;

			controller.ControllerContext.RouteData = routedData;

			// Headers
			var context = new DefaultHttpContext();

			if (addUserId)
				context.Request.Headers.Add("X-USER-ID", UserId);

			if (addUserRoles)
				context.Request.Headers.Add("X-USER-ROLES", UserRoles);

			if (addUserScopes)
				context.Request.Headers.Add("X-USER-SCOPES", UserScopes);

			controller.ControllerContext.HttpContext = context;

		}
	}
}
