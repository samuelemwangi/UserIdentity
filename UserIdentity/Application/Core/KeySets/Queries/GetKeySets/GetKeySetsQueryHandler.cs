using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Interfaces.Security;

namespace UserIdentity.Application.Core.KeySets.Queries.GetKeySets
{
    public record GetKeySetsQuery : BaseQuery
	{

	}
	public class GetKeySetsQueryHandler : IGetItemsQueryHandler<GetKeySetsQuery, IDictionary<String, IList<Dictionary<String, String>>>>
	{
		private readonly IKeySetFactory _keySetFactory;

		public GetKeySetsQueryHandler(IKeySetFactory keySetFactory)
		{
			_keySetFactory = keySetFactory;
		}

		public async Task<IDictionary<String, IList<Dictionary<String, String>>>> GetItemsAsync(GetKeySetsQuery query)
		{

			Dictionary<String, String> keySet = new()
			{
				{ "alg", _keySetFactory.GetAlgorithm() },
				{ "kty", _keySetFactory.GetKeyType() },
				{ "kid", _keySetFactory.GetKeyId() },
				{ "k", _keySetFactory.GetBase64URLEncodedSecretKey()}
			};

			List<Dictionary<String, String>> keySetList = new()
			{
				keySet
			};

			Dictionary<String, IList<Dictionary<String, String>>> keySets = new()
			{
				{"keys", keySetList},
			};

			return await Task.FromResult(keySets);

		}

	}
}
