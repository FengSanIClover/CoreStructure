
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace TP5.Core.NetCore.Interface
{
    public interface IExceptionLogService
    {
        void Log(ExceptionContext filterContext);

        void FallbackLog(ExceptionContext filterContext,Exception ex);
    }

    public class EmptyExceptionLogService : IExceptionLogService
    {
        public void FallbackLog(ExceptionContext filterContext, Exception ex)
        {
            //throw new NotImplementedException();
        }

        public void Log(ExceptionContext filterContext)
        {
            //throw new NotImplementedException();
        }
    }
}
