using System;
using System.Collections.Generic;
using System.Text;

namespace TP5.Core.NetCore.Results
{
    public class ApiReturnResult 
    {
        public object Result { get; set; }
        public string ReturnCode { get; set; }
        public string Message { get; set; }
        public string ExceptionData { get; set; }
    }
}
