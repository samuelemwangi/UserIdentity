using System;

using UserIdentity.Infrastructure.Utilities;

using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Utilities
{
	public class StringHelperTests
	{

		[Fact]
		public void Add_Spaces_ToSentence_Returns_Sentence_With_Spaces()
		{
			// Arrange
			String originalAString = "500 -InternalServerError",
						 expectedAString = "500 - Internal Server Error",
						 originalBString = "Failed badly",
						 expectedBString = "Failed badly";


			var stringHelper = new StringHelper();

			// Act & Assert
			Assert.Equal(String.Empty, stringHelper.AddSpacesToSentence("", true));
			Assert.Equal(expectedAString, stringHelper.AddSpacesToSentence(originalAString, true));
			Assert.Equal(expectedBString, stringHelper.AddSpacesToSentence(originalBString, true));
		}

	}
}

