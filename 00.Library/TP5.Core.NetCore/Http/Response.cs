using System;

namespace TP5.Core.NetCore.Http
{
    public class Response
    {
        public string ReturnCode { get; set; }

        public string Message { get; set; }

        public Exception ExceptionData { get; set; }
    }

    public class Response<T> : Response
    {
        public T Result { get; set; }

        // 轉 Json 需要有空建構子
        public Response()
        {
        }

        public Response(T result, string returnCode)
        {
            this.Result = result;
            this.ReturnCode = returnCode;
        }

        public Response(T result, string returnCode, string message) :
            this(result, returnCode)
        {
            this.Message = message;
        }
    }
}