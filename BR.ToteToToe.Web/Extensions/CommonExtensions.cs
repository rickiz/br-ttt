using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace BR.ToteToToe.Web.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// If DBNull or null value then return 0.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this object value)
        {
            return Convert.IsDBNull(value) || value.ToString2() == "" ? 0 : Convert.ToInt32(value);
        }

        /// <summary>
        /// If DBNull or null value then return 0.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int64 ToLong(this object value)
        {
            return Convert.IsDBNull(value) || value.ToString2() == "" ? 0 : Convert.ToInt64(value.ToString2().Trim());
        }

        /// <summary>
        /// If DBNull or null value then return empty string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString2(this object value)
        {
            return Convert.IsDBNull(value) || value == null ? "" : value.ToString();
        }

        /// <summary>
        /// Returns a copy of this string to lowercase.
        /// If DBNull or null value then return empty string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToLowerStr(this object value)
        {
            return value.ToString2().ToLower();
        }

        /// <summary>
        /// If DBNull or null value then return 0.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDouble(this object value)
        {
            return Convert.IsDBNull(value) || value == null ? 0 : Convert.ToDouble(value);
        }

        public static bool ToBool(this object value)
        {
            return Convert.IsDBNull(value) || value == null ? false : Convert.ToBoolean(value);
        }

        public static bool? ToNullableBool(this object value)
        {
            if (value == null || Convert.IsDBNull(value))
                return null;

            return Convert.ToBoolean(value);
        }

        public static int? ToNullableInt(this object value)
        {
            if (value == null || Convert.IsDBNull(value))
                return null;
            else
                return Convert.ToInt32(value);
        }

        public static Decimal ToDecimal(this object value)
        {
            return Convert.IsDBNull(value) || value == null || value.ToString2() == string.Empty ? 0 : Convert.ToDecimal(value);
        }

        public static DateTime ToDateTime(this object value)
        {
            //return Convert.IsDBNull(value) || value == null ? DateTime.MinValue : DateTime.ParseExact(value,"yyyyMMdd",CultureInfo.InvariantCulture);
            return Convert.IsDBNull(value) || value == null || string.IsNullOrEmpty(value.ToString2()) ? DateTime.MinValue : DateTime.ParseExact(value.ToString(), "yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
        }

        public static bool IsEmpty<T>(this IEnumerable<T> value)
        {
            if (value == null || value.Count() == 0)
                return true;

            return false;
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static string ToFormatString(this DateTime? target, string format)
        {
            if (!target.HasValue)
                return "";

            return target.Value.ToString(format);
        }
    }
}