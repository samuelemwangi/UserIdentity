using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UserIdentity.Application.Exceptions;
using Xunit;

namespace UserIdentity.UnitTests.Application.Exceptions
{
	public class MissingConfigurationExceptionTests
	{
		[Fact]
		public async Task Missing_Configuration_Item_Throws_MissingConfigurationException()
		{
			// Arrange
			var configItem = "Test";

			var expectedMessage = configItem + ": Configuration for item - " + configItem + " - is invalid";

			// Act
			var exception = await Assert.ThrowsAsync<MissingConfigurationException>(() => throw new MissingConfigurationException(configItem));

			// Assert
			Assert.Equal(exception.Message, expectedMessage);
		}
	}
}
