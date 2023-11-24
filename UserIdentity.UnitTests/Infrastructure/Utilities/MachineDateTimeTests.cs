using System;
using System.Globalization;

using UserIdentity.Infrastructure.Utilities;

using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Utilities
{
	public class MachineDateTimeTests
	{
		[Fact]
		public void Get_CurrentYear_Returns_CurrentYear()
		{
			Assert.IsType<Int32>(MachineDateTime.CurrentYear);
			Assert.Equal(DateTime.UtcNow.Year, MachineDateTime.CurrentYear);
		}

		[Fact]
		public void Get_CurrentMonth_Returns_CurrentMonth()
		{
			Assert.IsType<Int32>(MachineDateTime.CurrentMonth);
			Assert.Equal(DateTime.UtcNow.Month, MachineDateTime.CurrentMonth);
		}

		[Fact]
		public void Get_DefaultNull_Returns_DefaultNull()
		{
			// Arrange
			var machineDateTime = new MachineDateTime();

			// Assert
			Assert.Null(machineDateTime.DefaultNull);
		}

		[Fact]
		public void Get_TimeStamp_Returns_TimeStamp()
		{
			// Arrange
			var machineDateTime = new MachineDateTime();

			var currentUTCTime = DateTime.UtcNow;

			// Act
			var timeStamp = machineDateTime.GetTimeStamp();
			var actualUTCTime = DateTime.ParseExact(timeStamp, "yyyyMMddHHmmssffff", CultureInfo.InvariantCulture);

			// Assert
			Assert.NotNull(timeStamp);

			Assert.Equal(currentUTCTime.Year, actualUTCTime.Year);
			Assert.Equal(currentUTCTime.Month, actualUTCTime.Month);
			Assert.Equal(currentUTCTime.Day, actualUTCTime.Day);
			Assert.Equal(currentUTCTime.Hour, actualUTCTime.Hour);
			Assert.Equal(currentUTCTime.Minute, actualUTCTime.Minute);
		}

		[Fact]
		public void Resolve_Date_Returns_ResolvedDate()
		{
			// Arrange
			var machineDateTime = new MachineDateTime();
			var curentDate = DateTime.Now;

			// Action & Assert
			Assert.Equal("", machineDateTime.ResolveDate(null));
			Assert.Equal(curentDate.ToLongDateString(), machineDateTime.ResolveDate(curentDate));
		}


		[Fact]
		public void To_UnixEpochDate_Returns_Epoch()
		{
			// Arrange
			var machineDateTime = new MachineDateTime();
			var curentDate = DateTime.Now;

			// Act & Assert
			Assert.IsType<Int64>(machineDateTime.ToUnixEpochDate(curentDate));
		}


	}
}

