using UserIdentity.Application.Interfaces.Security;

namespace UserIdentity.Application.Core.KeySets.Queries.GetKeySets
{
	public record GetKeySetsQuery : BaseQuery
	{

	}
	public class GetKeySetsQueryHandler
	{
		private readonly IKeySetFactory _keySetFactory;

		public GetKeySetsQueryHandler(IKeySetFactory keySetFactory)
		{
			_keySetFactory = keySetFactory;
		}

		public Dictionary<string, IList<Dictionary<string, string>>> GetKeySets(GetKeySetsQuery query)
		{

			Dictionary<string, string> keySet = new()
			{
				{ "alg", _keySetFactory.GetAlgorithm() },
				{ "kty", _keySetFactory.GetKeyType() },
				{ "kid", _keySetFactory.GetKeyId() },
				{ "k", _keySetFactory.GetBase64URLEncodedSecretKey()}
			};

			List<Dictionary<string, string>> keySetList = new()
			{
				keySet
			};

			Dictionary<string, IList<Dictionary<string, string>>> keySets = new()
			{
				{"keys", keySetList},
			};

			return keySets;

		}
	}
}
