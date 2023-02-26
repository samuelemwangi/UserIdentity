using System;
using System.ComponentModel;

using UserIdentity.Application.Enums;

using Xunit;

namespace UserIdentity.UnitTests.Application.Enums
{
	public class EnumExtensionsTests
	{
		enum TestEnum
		{
			FAILS,

			[Description("Label desc for enum")]
			PASSES
		}

		[Fact]
		public void Get_DisplayName_Returns_Enum_Display_Name()
		{
			Assert.Equal(String.Empty, TestEnum.FAILS.GetDisplayName());
			Assert.Equal("Label desc for enum", TestEnum.PASSES.GetDisplayName());

		}

	}
}

