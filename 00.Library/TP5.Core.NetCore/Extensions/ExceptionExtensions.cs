using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace TP5.Core.NetCore.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetInnerErrorMessage(this WebException webEx)
        {
            var webExResponse = webEx.Response as HttpWebResponse;
            if (webExResponse.StatusCode == HttpStatusCode.InternalServerError)
            {
                using (var webStream = webExResponse.GetResponseStream())
                using (var resultStream = new StreamReader(webStream, Encoding.GetEncoding("big5")))
                {
                    return resultStream.ReadToEnd();
                }
            }

            return webEx.Message;
        }

        public static string GetInnerErrorMessage(this Exception exception)
        {
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            return exception.Message;
        }
    }
}
