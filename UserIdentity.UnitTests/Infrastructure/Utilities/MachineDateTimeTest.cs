using System;
using System.Globalization;
using UserIdentity.Infrastructure.Utilities;
using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Utilities
{
    public class MachineDateTimeTest
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
            MachineDateTime machineDateTime = new MachineDateTime();
            Assert.Null(machineDateTime.DefaultNull);
        }

        [Fact]
        public void Get_TimeStamp_Returns_TimeStamp()
        {
            MachineDateTime machineDateTime = new MachineDateTime();

            DateTime currentUTCTime = DateTime.UtcNow;

            String timeStamp = machineDateTime.GetTimeStamp();
            DateTime actualUTCTime = DateTime.ParseExact(timeStamp, "yyyyMMddHHmmssffff", CultureInfo.InvariantCulture);


            Assert.NotNull(timeStamp);

            Assert.Equal(currentUTCTime.Year, actualUTCTime.Year);
            Assert.Equal(currentUTCTime.Month, actualUTCTime.Month);
            Assert.Equal(currentUTCTime.Day, actualUTCTime.Day);
            Assert.Equal(currentUTCTime.Hour, actualUTCTime.Hour);
            Assert.Equal(currentUTCTime.Minute, actualUTCTime.Minute);
        }

        [Fact]
        public void ResolveDate_Returns_ResolvedDate()
        {
            MachineDateTime machineDateTime = new MachineDateTime();
            DateTime curentDate = DateTime.Now;

            Assert.Equal("", machineDateTime.ResolveDate(null));
            Assert.Equal(curentDate.ToLongDateString(), machineDateTime.ResolveDate(curentDate));
        }


        [Fact]
        public void ToUnixEpochDate_Returns_Epoch()
        {
            MachineDateTime machineDateTime = new MachineDateTime();
            DateTime curentDate = DateTime.Now;

            Assert.IsType<Int64>(machineDateTime.ToUnixEpochDate(curentDate));
        }


    }
}

