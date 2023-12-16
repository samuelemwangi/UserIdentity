using Microsoft.AspNetCore.Mvc;

namespace UserIdentity.Presentation.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public abstract class BaseController : ControllerBase
	{
		protected string? LoggedInUserId => Request.Headers.Where(h => h.Key.ToUpper().Equals("X-USER-ID")).Select(x => x.Value).FirstOrDefault();
		protected string? UserRoleClaims => Request.Headers.Where(h => h.Key.ToUpper().Equals("X-USER-ROLES")).Select(x => x.Value).FirstOrDefault();
		protected string? UserScopeClaims => Request.Headers.Where(h => h.Key.ToUpper().Equals("X-USER-SCOPES")).Select(x => x.Value).FirstOrDefault();

		protected string? EntityName => this.ControllerContext.RouteData.Values["controller"]?.ToString();
	}
}
