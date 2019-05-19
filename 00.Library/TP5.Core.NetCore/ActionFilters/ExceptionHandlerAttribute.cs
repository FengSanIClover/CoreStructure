using TP5.Core.NetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TP5.Core.NetCore.ActionFilters
{
    public class ExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        public string ReturnCode = ReturnCodes.CODE_FAILURE;
        public string Message = MsgCodes.Msg_99;

        //public override void OnException(HttpActionExecutedContext filterContext)
        //{
        //    string controllerName = filterContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
        //    string actionName = filterContext.ActionContext.ActionDescriptor.ActionName;

        //    var baseResponse = new Response();
        //    baseResponse.ReturnCode = ReturnCode;
        //    baseResponse.Message = Message;

        //    if (filterContext.Exception is BusinessException)
        //    {
        //        var businessEx = filterContext.Exception as BusinessException;

        //        if (!string.IsNullOrEmpty(businessEx.ErrorCode))
        //        {
        //            baseResponse.ReturnCode = businessEx.ErrorCode;
        //        }

        //        if (!string.IsNullOrEmpty(businessEx.ErrorMsg))
        //        {
        //            baseResponse.Message = businessEx.ErrorMsg;
        //        }

        //        //baseResponse.ExceptionData = filterContext.Exception;
        //    }
        //    else
        //    {
        //        //TODO:之後要寫入 Log
        //        //logger.Error($"{controllerName}/{actionName}/{filterContext.Exception.ToString()}");
        //        baseResponse.ExceptionData = filterContext.Exception;
        //    }

        //    filterContext.Response = filterContext.Request.CreateResponse(baseResponse);

        //    base.OnException(filterContext);
        //}

        public override void OnException(ExceptionContext context)
        {
            var items = context.HttpContext.Items;
            if (items.ContainsKey("Tracking"))
            {
                //Tracking tracking = (Tracking)items["Tracking"];
                //Tracking tracking = items["Tracking"];
                //tracking.Exception(context.Exception, true);
            }

            base.OnException(context);
        }
    }
}