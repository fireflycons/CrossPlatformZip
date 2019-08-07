namespace Firefly.CrossPlatformZip.Tests.Unit
{
    using System;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The unix time tests.
    /// </summary>
    [TestClass]
    public class UnixTimeTests
    {
        /// <summary>
        /// The convert date time to unix time and back gives input value.
        /// </summary>
        [TestMethod]
        public void ConvertDateTimeToUnixTimeAndBack_GivesInputValue()
        {
            var datetime = new DateTime(2038, 1, 1);

            var ut = datetime.ToUnixTime();
            var dt = ut.FromUnixTime();

            dt.Should().Be(datetime);
        }
    }
}