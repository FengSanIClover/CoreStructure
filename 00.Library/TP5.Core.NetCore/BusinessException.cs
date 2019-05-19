using System;

namespace TP5.Core.NetCore
{
    public class BusinessException : Exception
    {
        public string ErrorMsg { get; set; }
        public string ErrorCode { get; set; }

        public BusinessException(string errorCode = "", string errorMsg = "")
        {
            this.ErrorMsg = errorMsg;
            this.ErrorCode = errorCode;
        }
    }
}