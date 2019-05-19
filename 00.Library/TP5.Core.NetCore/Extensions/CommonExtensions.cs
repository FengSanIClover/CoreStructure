
using System;

namespace TP5.Core.NetCore.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// yyyy/MM/dd HH:mm:ss
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string ToFullDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy/MM/dd HH:mm:ss");
        }

        /// <summary>
        /// 擷取固定字串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="digit"></param>
        /// <returns></returns>
        public static string SubstringLimit(this string str, int digit)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            if (str.Length <= digit)
                return str;

            return str.Substring(0, digit);
        }
    }
}
