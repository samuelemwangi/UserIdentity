using System.Net;

using Microsoft.AspNetCore.Mvc;

using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.KeySets.Queries.GetKeySets;

namespace UserIdentity.Presentation.Controllers.Security
{
	public class JWKSController : BaseController
	{
		private readonly IGetItemsQueryHandler<GetKeySetsQuery, IDictionary<string, IList<Dictionary<string, string>>>> _getKeySetsQueryHandler;

		public JWKSController(IGetItemsQueryHandler<GetKeySetsQuery, IDictionary<string, IList<Dictionary<string, string>>>> getKeySetsQueryHandler)
		{
			_getKeySetsQueryHandler = getKeySetsQueryHandler;
		}

		[HttpGet]
		[Route("keys")]
		public async Task<IActionResult> GetKeySetsAsync()
		{
			var keySets = await _getKeySetsQueryHandler.GetItemsAsync(new GetKeySetsQuery { });
			return StatusCode((int)HttpStatusCode.OK, keySets);
		}
	}
}
