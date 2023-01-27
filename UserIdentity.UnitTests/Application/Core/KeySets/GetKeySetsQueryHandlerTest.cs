﻿using FakeItEasy;
using UserIdentity.Application.Core.KeySets.Queries.GetKeySets;
using UserIdentity.Application.Interfaces.Security;
using Xunit;

namespace UserIdentity.UnitTests.Application.Core.KeySets
{
    public class GetKeySetsQueryHandlerTest
    {
        private readonly IKeySetFactory _keySetFactory;

        public GetKeySetsQueryHandlerTest()
        {
            _keySetFactory = A.Fake<IKeySetFactory>();
        }

        [Fact]
        public void Get_KeySets_Returns_Keysets()
        {
            GetKeySetsQuery query = new GetKeySetsQuery
            {

            };

            GetKeySetsQueryHandler getKeySetsQueryHandler = new GetKeySetsQueryHandler(_keySetFactory);

            var keySets = getKeySetsQueryHandler.GetKeySets(query);

            Assert.NotNull(keySets);
            Assert.Single(keySets);

            Assert.Single(keySets?["keys"]);

            Assert.NotNull(keySets?["keys"][0]["alg"]);
            Assert.NotNull(keySets?["keys"][0]["kty"]);
            Assert.NotNull(keySets?["keys"][0]["kid"]);
            Assert.NotNull(keySets?["keys"][0]["k"]);
        }

    }
}
