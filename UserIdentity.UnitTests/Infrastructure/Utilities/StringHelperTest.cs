using System;
using UserIdentity.Infrastructure.Utilities;
using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Utilities
{
    public class StringHelperTest
    {

        [Fact]
        public void AddSpacesToSentence_Returns_Sentence_With_Spaces()
        {
            String originalAString = "500 -InternalServerError",
                   expectedAString = "500 - Internal Server Error",
                   originalBString = "Failed badly",
                   expectedBString = "Failed badly";


            StringHelper stringHelper = new StringHelper();

            Assert.Equal(expectedAString, stringHelper.AddSpacesToSentence(originalAString, true));
            Assert.Equal(expectedBString, stringHelper.AddSpacesToSentence(originalBString, true));
        }

    }
}

