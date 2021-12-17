namespace Firefly.CrossPlatformZip.Tests.Unit
{
    using System;

    using FluentAssertions;

    using Xunit;

    /// <summary>
    /// The unix time tests.
    /// </summary>
    public class UnixTimeTests
    {
        /// <summary>
        /// The convert date time to unix time and back gives input value.
        /// </summary>
        [Fact]
        public void ConvertDateTimeToUnixTimeAndBack_GivesInputValue()
        {
            var datetime = new DateTime(2038, 1, 1);

            var ut = datetime.ToUnixTime();
            var dt = ut.FromUnixTime();

            dt.Should().Be(datetime);
        }
    }
}