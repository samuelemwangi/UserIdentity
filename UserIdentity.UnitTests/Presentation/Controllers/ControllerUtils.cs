﻿using System;
using System.Collections.Generic;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using PolyzenKit.Infrastructure.Security.Jwt;
using PolyzenKit.Presentation.Controllers;

namespace UserIdentity.UnitTests.Presentation.Controllers;

internal static class ControllerUtils
{

	public static void UpdateContext(
		this BaseController controller,
		string? controllerName,
		bool addUserId = false,
		string? userId = null,
		bool addUserRoles = false,
		string? userRoles = null,
		bool addUserScopes = false,
		string? userScopes = null
	 )
	{
		var routedData = new RouteData();
		routedData.Values["controller"] = controllerName;

		controller.ControllerContext.RouteData = routedData;

		var context = new DefaultHttpContext();

		var claims = new List<Claim>();

		if (addUserId)
			claims.Add(new Claim(JwtCustomClaimNames.Id, userId!));

		if (addUserRoles)
			foreach (var role in userRoles!.Split(","))
				claims.Add(new Claim(JwtCustomClaimNames.Rol, role));

		if (addUserScopes)
			foreach (var scope in userScopes!.Split(","))
				claims.Add(new Claim(JwtCustomClaimNames.Scope, scope));

		context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));

		controller.ControllerContext.HttpContext = context;

	}
}
