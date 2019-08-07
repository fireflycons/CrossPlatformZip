namespace Firefly.CrossPlatformZip
{
    using System;

    /// <summary>
    /// The date time extensions.
    /// </summary>
    internal static class DateTimeExtensions
    {
        /// <summary>
        /// Convert integer unix time to datetime
        /// </summary>
        /// <param name="unixTime">
        /// The unix time.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime FromUnixTime(this int unixTime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) + new TimeSpan(unixTime * 10000000L);
        }

        /// <summary>
        /// Convert datetime to integer unix time.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ToUnixTime(this DateTime dateTime)
        {
            return (int)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
    }
}