using System;

namespace Iso8601ExtMethodLib
{
    public static class Iso8601Ext
    {
        const string ISO8601format = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK";

        /// <summary>
        /// Produce ISO 8601 format time string
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="IsUTC">Set to true if input datetime is UTC time</param>
        /// <returns></returns>
        public static string ToIso8601String(this DateTime dateTime, bool IsUTC = false)
        {
            if (!IsUTC)
            {
                dateTime = dateTime.ToUniversalTime();
            }

            return dateTime.ToString(ISO8601format) + "Z";
        }

        /// <summary>
        /// Parse ISO 8601 format string to Datetime
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime FromIso8601String(this string input)
        {
            return ParseIso8601(input).DateTime;
        }

        private static DateTimeOffset ParseIso8601(string iso8601String)
        {
            var input = iso8601String.Substring(0, iso8601String.Length - 1);

            return DateTimeOffset.ParseExact(
                input,
                new string[] { ISO8601format },
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None);
        }
    }
}
