using System;
using System.Text;

namespace UnityEssentials.Extensions
{
    /// <summary>
    /// Extensions for DateTime
    /// </summary>
    public static class ExtDateTime
    {
        public struct TimeSec
        {
            public int Years;
            public int Days;
            public int Hours;
            public int Minutes;
            public int Seconds;
            public float Miliseconds;

            public TimeSec(int minutes, int seconds, float miliseconds)
                : this()
            {
                Minutes = minutes;
                Seconds = seconds;
                Miliseconds = miliseconds;
            }

            public TimeSec(int hours, int minutes, int seconds, float miliseconds)
                : this()
            {
                Hours = hours;
                Minutes = minutes;
                Seconds = seconds;
                Miliseconds = miliseconds;
            }

            public TimeSec(int days, int hours, int minutes, int seconds, float miliseconds)
                : this()
            {
                Days = days;
                Hours = hours;
                Minutes = minutes;
                Seconds = seconds;
                Miliseconds = miliseconds;
            }

            public TimeSec(int years, int days, int hours, int minutes, int seconds, float miliseconds)
            {
                Years = years;
                Days = days;
                Hours = hours;
                Minutes = minutes;
                Seconds = seconds;
                Miliseconds = miliseconds;
            }
        }

        /// <summary>
        /// from a TimeSec, transform it to a string
        /// </summary>
        /// <param name="timeSec"></param>
        /// <returns></returns>
        public static string GetTimeInStringFromTimeSec(TimeSec timeSec)
        {
            StringBuilder timeInText = new StringBuilder();
            if (timeSec.Seconds == -1)
            {
                return (timeInText.ToString());
            }

            if (timeSec.Minutes < 10)
            {
                timeInText.Append("0");
            }
            timeInText.Append(timeSec.Minutes);

            timeInText.Append(" ' ");

            if (timeSec.Seconds < 10)
            {
                timeInText.Append("0");
            }
            timeInText.Append(timeSec.Seconds);

            timeInText.Append(" '' ");

            timeSec.Miliseconds = (int)timeSec.Miliseconds;
            if (timeSec.Miliseconds < 10)
            {
                timeInText.Append("0");
            }

            if (timeSec.Miliseconds >= 100)
            {
                timeSec.Miliseconds = 99;
            }

            timeInText.Append(timeSec.Miliseconds);
            return (timeInText.ToString());
        }

        /// <summary>
        /// Get a TimeSec from a float time in seconds
        /// </summary>
        /// <param name="time">time in seconds</param>
        /// <returns></returns>
        public static TimeSec GetTimeSecFromMinutsToSeconds(float time)
        {
            int minutes = GetMinutes(time);
            int second = GetSeconds(time);
            float miliseconds = GetMiliseconds(time);
            return (new TimeSec(minutes, second, miliseconds));
        }

        /// <summary>
        /// Get a TimeSec from a float time in seconds
        /// </summary>
        /// <param name="time">time in seconds</param>
        /// <returns></returns>
        public static TimeSec GetTimeSecFromDaysToSeconds(float time)
        {
            int days = GetDays(time);
            int hours = GetHours(time) - (days * 24);
            int minutes = GetMinutes(time) - (hours * 60) - (days * 24 * 60);

            int second = GetSeconds(time);
            float miliseconds = GetMiliseconds(time);
            return (new TimeSec(days, hours, minutes, second, miliseconds));
        }

        /// <summary>
        /// transform a time float to a string minutes/seconds/miliseconds
        /// </summary>
        /// <param name="time">time in seconds</param>
        /// <returns></returns>
        public static string GetTimeString(float time)
        {
            return (GetTimeInStringFromTimeSec(GetTimeSecFromMinutsToSeconds(time)));
        }

        /// <summary>
        /// get the current Date
        /// </summary>
        /// <returns></returns>
        public static string GetDateNow()
        {
            return System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        }

        public static string ConvertSeconds(int time)
        {
            var _minutes = (int)(time / 60);
            string minutes = "";
            int _seconds = (time % 60);
            string seconds = "";
            if (_minutes > 0) minutes = _minutes + " minute" + (_minutes > 1 ? "s " : " ");
            if (_seconds > 0) seconds = _seconds + " second" + (_seconds > 1 ? "s " : " ");
            return (minutes + seconds).Substring(0, (minutes + seconds).Length - 1);
        }

        /// <summary>
        /// from a given time in seconds, Get X
        /// </summary>

        public static int GetDays(float time)
        {
            int days = (int)(time / 3600 / 24);
            return (days);
        }

        public static int GetHours(float time)
        {
            int hours = (int)(time / 3600);
            return (hours);
        }

        public static int GetMinutes(float time)
        {
            int minutes = (int)(time / 60);
            return (minutes);
        }
        public static int GetSeconds(float time)
        {
            int seconds = (int)(time % 60);
            return (seconds);
        }
        public static float GetMiliseconds(float time)
        {
            int minutes = GetMinutes(time);
            int seconds = (int)(time % 60);
            return ((time - seconds - (minutes * 60)) * 100f);
        }

        #region Constants

        /// <summary>
        /// Constant that represents number of days in a week
        /// </summary>
        internal const int DAYS_PER_WEEK = 7;

        /// <summary>
        /// Constant that represents number of days in a fortnight
        /// </summary>
        internal const int DAYS_PER_FORTNIGHT = DAYS_PER_WEEK * 2;

        /// <summary>
        /// Constant that represents number of weeks in a fortnight
        /// </summary>
        internal const int WEEKS_PER_FORTNIGHT = 2;

        /// <summary>
        /// Constant that represents number of years in a decade
        /// </summary>
        internal const int YEARS_PER_DECADE = 10;

        /// <summary>
        /// Constant that represents number of years in a century
        /// </summary>
        internal const int YEARS_PER_CENTURY = 100;

        /// <summary>
        /// Constant that represents January month
        /// </summary>
        private const int JANUARY = 1;

        /// <summary>
        /// Constant that represents February month
        /// </summary>
        private const int FEBRUARY = 2;

        /// <summary>
        /// Constant that represents March month
        /// </summary>
        private const int MARCH = 3;

        /// <summary>
        /// Constant that represents April month
        /// </summary>
        private const int APRIL = 4;

        /// <summary>
        /// Constant that represents May month
        /// </summary>
        private const int MAY = 5;

        /// <summary>
        /// Constant that represents June month
        /// </summary>
        private const int JUNE = 6;

        /// <summary>
        /// Constant that represents July month
        /// </summary>
        private const int JULY = 7;

        /// <summary>
        /// Constant that represents August month
        /// </summary>
        private const int AUGUST = 8;

        /// <summary>
        /// Constant that represents September month
        /// </summary>
        private const int SEPTEMBER = 9;

        /// <summary>
        /// Constant that represents October month
        /// </summary>
        private const int OCTOBER = 10;

        /// <summary>
        /// Constant that represents November month
        /// </summary>
        private const int NOVEMBER = 11;

        /// <summary>
        /// Constant that represents December month
        /// </summary>
        private const int DECEMBER = 12;

        // Constants
        #endregion

        #region GetString

        /// <summary>
        /// Gets the day string of the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation of day.</returns>
        /// <example>Returns "Sunday" for the date 1.1.2012.</example>
        public static string GetDayString(this DateTime date)
        {
            return date.DayOfWeek.ToString();
        }

        /// <summary>
        /// Gets the month string of the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation of month.</returns>
        /// <example>Returns "January" for the date 1.1.2012.</example>
        public static string GetMonthString(this DateTime date)
        {
            return date.ToString("MMMM");
        }

        // GetString
        #endregion

        #region DD/MM/YY methods

        /// <summary>
        /// Formats the given DateTime to "dd/MM/yy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01/01/12" for the date 1.1.2012.</example>
        public static string ToDdMmYySlash(this DateTime date)
        {
            return date.ToString("dd/MM/yy");
        }

        /// <summary>
        /// Formats the given DateTime to "dd.MM.yy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01.01.12" for the date 1.1.2012.</example>
        public static string ToDdMmYyDot(this DateTime date)
        {
            return date.ToString("dd.MM.yy");
        }

        /// <summary>
        /// Formats the given DateTime to "dd-MM-yy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01-01-12" for the date 1.1.2012.</example>
        public static string ToDdMmYyHyphen(this DateTime date)
        {
            return date.ToString("dd-MM-yy");
        }

        /// <summary>
        /// Formats the given DateTime to "ddMMyy" by applying the given separator.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="separator">The given separator.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01,01,12" for the date 1.1.2012 and separator ,.</example>
        public static string ToDdMmYyWithSep(this DateTime date, string separator)
        {
            return date.ToString(string.Format("dd{0}MM{0}yy", separator));
        }

        #endregion

        #region DD/MM/YYYY methods

        /// <summary>
        /// Formats the given DateTime to "dd/MM/yyyy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01/01/2012" for the date 1.1.2012.</example>
        public static string ToDdMmYyyySlash(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Formats the given DateTime to "dd.MM.yyyy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01.01.2012" for the date 1.1.2012.</example>
        public static string ToDdMmYyyyDot(this DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// Formats the given DateTime to "dd-MM-yyyy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01-01-2012" for the date 1.1.2012.</example>
        public static string ToDdMmYyyyHyphen(this DateTime date)
        {
            return date.ToString("dd-MM-yyyy");
        }

        /// <summary>
        /// Formats the given DateTime to "ddMMyyyy" by applying the given separator.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="separator">The given separator.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01,01,2012" for the given DateTime 1.1.2012 and separator ,.</example>
        public static string ToDdMmYyyyWithSep(this DateTime date, string separator)
        {
            return date.ToString(string.Format("dd{0}MM{0}yyyy", separator));
        }

        #endregion

        #region MM/DD/YY methods

        /// <summary>
        /// Formats the given DateTime to "MM/dd/yy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12/01/12" for the date 1.12.2012.</example>
        public static string ToMmDdYySlash(this DateTime date)
        {
            return date.ToString("MM/dd/yy");
        }

        /// <summary>
        /// Formats the given DateTime to "MM.dd.yy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12.01.12" for the date 1.12.2012.</example>
        public static string ToMmDdYyDot(this DateTime date)
        {
            return date.ToString("MM.dd.yy");
        }

        /// <summary>
        /// Formats the given DateTime to "MM-dd-yy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12-01-12" for the date 1.12.2012.</example>
        public static string ToMmDdYyHyphen(this DateTime date)
        {
            return date.ToString("MM-dd-yy");
        }

        /// <summary>
        /// Formats the given DateTime to "MMddyy" by applying the given separator.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="separator">The given separator.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12,01,12" for the given DateTime1.12.2012 and separator ,.</example>
        public static string ToMmDdYyWithSep(this DateTime date, string separator)
        {
            return date.ToString(string.Format("MM{0}dd{0}yy", separator));
        }

        #endregion

        #region MM/DD/YYYY methods

        /// <summary>
        /// Formats the given DateTime to "MM/dd/yyyy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12/01/2012" for the date 1.12.2012.</example>
        public static string ToMmDdYyyySlash(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy");
        }

        /// <summary>
        /// Formats the given DateTime to "MM.dd.yyyy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12.01.2012" for the date 1.12.2012.</example>
        public static string ToMmDdYyyyDot(this DateTime date)
        {
            return date.ToString("MM.dd.yyyy");
        }

        /// <summary>
        /// Formats the given DateTime to "MM-dd-yyyy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12-01-2012" for the date 1.12.2012.</example>
        public static string ToMmDdYyyyHyphen(this DateTime date)
        {
            return date.ToString("MM-dd-yyyy");
        }

        /// <summary>
        /// Formats the given DateTime to "MMddyyyy" by applying the given separator.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="separator">The given separator.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12,01,2012" for the given DateTime1.12.2012 and separator ,.</example>
        public static string ToMmDdYyyyWithSep(this DateTime date, string separator)
        {
            return date.ToString(string.Format("MM{0}dd{0}yyyy", separator));
        }

        #endregion

        #region YY/MM/DD methods

        /// <summary>
        /// Formats the given DateTime to "yy/MM/dd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12/11/30" for the date 30.11.2012.</example>
        public static string ToYyMmDdSlash(this DateTime date)
        {
            return date.ToString("yy/MM/dd");
        }

        /// <summary>
        /// Formats the given DateTime to "yy.MM.dd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12.11.30" for the date 30.11.2012.</example>
        public static string ToYyMmDdDot(this DateTime date)
        {
            return date.ToString("yy.MM.dd");
        }

        /// <summary>
        /// Formats the given DateTime to "yy-MM-dd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12-11-30" for the date 30.11.2012.</example>
        public static string ToYyMmDdHyphen(this DateTime date)
        {
            return date.ToString("yy-MM-dd");
        }

        /// <summary>
        /// Formats the given DateTime to "yyMMdd" by applying the given separator.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="separator">The given separator.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12,11,30" for the given DateTime30.11.2012 and separator ,.</example>
        public static string ToYyMmDdWithSep(this DateTime date, string separator)
        {
            return date.ToString(string.Format("yy{0}MM{0}dd", separator));
        }

        #endregion

        #region YYYY/MM/DD methods

        /// <summary>
        /// Formats the given DateTime to "yyyy/MM/dd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "2012/11/30" for the date 30.11.2012.</example>          
        public static string ToYyyyMmDdSlash(this DateTime date)
        {
            return date.ToString("yyyy/MM/dd");
        }

        /// <summary>
        /// Formats the given DateTime to "yyyy.MM.dd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "2012.11.30" for the date 30.11.2012.</example>           
        public static string ToYyyyMmDdDot(this DateTime date)
        {
            return date.ToString("yyyy.MM.dd");
        }

        /// <summary>
        /// Formats the given DateTime to "yyyy-MM-dd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "2012-11-30" for the date 30.11.2012.</example>           
        public static string ToYyyyMmDdHyphen(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Formats the given DateTime to "yyyyMMdd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="separator">The given separator.</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "2012,11,30" for the given DateTime30.11.2012 and separator ,.</example>           
        public static string ToYyyyMmDdWithSep(this DateTime date, string separator)
        {
            return date.ToString(string.Format("yyyy{0}MM{0}dd", separator));
        }

        #endregion

        public static long GetDateAsLong()
        {
            long time = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            return (time);
        }

        public static int GetDateAsInt()
        {
            long time = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            return ((int)time);
        }
    }
}