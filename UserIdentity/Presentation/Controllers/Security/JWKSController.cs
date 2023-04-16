using System.Net;

using Microsoft.AspNetCore.Mvc;
using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.KeySets.Queries.GetKeySets;

namespace UserIdentity.Presentation.Controllers.Security
{
    public class JWKSController : BaseController
	{
		private readonly IGetItemsQueryHandler<GetKeySetsQuery, IDictionary<String, IList<Dictionary<String, String>>>> _getKeySetsQueryHandler;

		public JWKSController(IGetItemsQueryHandler<GetKeySetsQuery, IDictionary<String, IList<Dictionary<String, String>>>> getKeySetsQueryHandler)
		{
			_getKeySetsQueryHandler = getKeySetsQueryHandler;
		}

		[HttpGet]
		[Route("keys")]
		public async Task<IActionResult> GetKeySetsAsync()
		{
			var keySets = await _getKeySetsQueryHandler.GetItemsAsync(new GetKeySetsQuery { });
			return StatusCode((Int32)HttpStatusCode.OK, keySets);
		}
	}
}
