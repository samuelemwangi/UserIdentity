using Microsoft.AspNetCore.Mvc;

using UserIdentity.Application.Core.KeySets.Queries.GetKeySets;

namespace UserIdentity.Presentation.Controllers.Security
{
	public class JWKSController : BaseController
	{
		private readonly GetKeySetsQueryHandler _getKeySetsQueryHandler;

		public JWKSController(GetKeySetsQueryHandler getKeySetsQueryHandler)
		{
			_getKeySetsQueryHandler = getKeySetsQueryHandler;
		}

		[HttpGet]
		[Route("keys")]
		public IActionResult GetKeySets()
		{
			var keySets = _getKeySetsQueryHandler.GetKeySets(new GetKeySetsQuery { });
			return Ok(keySets);
		}
	}
}
