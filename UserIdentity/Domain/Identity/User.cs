﻿using PolyzenKit.Domain.Entity;

namespace UserIdentity.Domain.Identity;

public record User : BaseAuditableEntity<string>
{
	public string? FirstName { get; internal set; }

	public string? LastName { get; internal set; }

	public string? EmailConfirmationToken { get; internal set; }

	public string? ForgotPasswordToken { get; internal set; }
}
