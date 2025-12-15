namespace UserIdentity.Application.Core.Users.Settings;

public record GoogleRecaptchaSettings
{
    public bool Enabled { get; set; }
    public string? SiteKey { get; set; }
}
