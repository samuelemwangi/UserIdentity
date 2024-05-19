using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Interfaces.Security;

namespace UserIdentity.Application.Core.KeySets.Queries.GetKeySets
{
	public record GetKeySetsQuery : BaseQuery
	{

	}
	public class GetKeySetsQueryHandler : IGetItemsQueryHandler<GetKeySetsQuery, IDictionary<string, IList<Dictionary<string, string>>>>
	{
		private readonly IKeySetFactory _keySetFactory;

		public GetKeySetsQueryHandler(IKeySetFactory keySetFactory)
		{
			_keySetFactory = keySetFactory;
		}

		public async Task<IDictionary<string, IList<Dictionary<string, string>>>> GetItemsAsync(GetKeySetsQuery query)
		{
			(var modulus, var exponent) = await _keySetFactory.GetModulusAndExponentForPublicKeyAsync();

			Dictionary<string, string> keySet = new()
			{
				{ "alg", _keySetFactory.GetAlgorithm() },
				{ "kty", _keySetFactory.GetKeyType() },
				{ "kid", _keySetFactory.GetKeyId() },
				{ "n", modulus},
				{ "e", exponent}
			};

			List<Dictionary<string, string>> keySetList = new()
			{
				keySet
			};

			Dictionary<string, IList<Dictionary<string, string>>> keySets = new()
			{
				{"keys", keySetList},
			};

			return await Task.FromResult(keySets);

		}

	}
}
