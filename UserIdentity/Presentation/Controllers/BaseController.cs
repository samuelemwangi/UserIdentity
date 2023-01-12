using Microsoft.AspNetCore.Mvc;

namespace UserIdentity.Presentation.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public abstract class BaseController : ControllerBase
	{
		protected String? LoggedInUserId => Request.Headers.Where(h => h.Key.ToUpper().Equals("X-USER-ID")).Select(x=>x.Value).FirstOrDefault();
		protected String? UserRoleClaims => Request.Headers.Where(h => h.Key.ToUpper().Equals("X-USER-ROLES")).Select(x => x.Value).FirstOrDefault();
        protected String? UserScopeClaims => Request.Headers.Where(h => h.Key.ToUpper().Equals("X-USER-SCOPES")).Select(x => x.Value).FirstOrDefault();

        protected String? EntityName => this.ControllerContext.RouteData.Values["controller"]?.ToString();

	}
}
