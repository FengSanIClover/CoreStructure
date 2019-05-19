using Microsoft.AspNetCore.Mvc;
using TP5.Core.NetCore.ActionFilters;
using TP5.Core.NetCore.Helper;
using TP5.Core.NetCore.Http;
using TP5.Core.NetCore.Results;

namespace TP5.Core.NetCore.Controllers
{
    [ExceptionHandler]
    public abstract class BaseApiController : ControllerBase
    {
        //#region Success Result => ReturnCode = 00

        //protected internal virtual ResponseResult Success(string message = "")
        //{

        //    return new ResponseResult(ReturnCodes.CODE_SUCCESS, message, RequestTranscriptHelpers.ToHttpRequestMessage(Request));
        //}

        //protected internal virtual ResponseResult<T> Success<T>(T content, string message = "")
        //{
        //    return new ResponseResult<T>(ReturnCodes.CODE_SUCCESS, message, content, RequestTranscriptHelpers.ToHttpRequestMessage(Request));
        //}

        //#endregion

        //#region Failure Result => ReturnCode = 99 or other

        //protected internal virtual ResponseResult Failure(string message = "")
        //{
        //    return new ResponseResult(ReturnCodes.CODE_FAILURE, message, RequestTranscriptHelpers.ToHttpRequestMessage(Request));
        //}

        //protected internal virtual ResponseResult FailureCode(string returnCode, string message = "")
        //{
        //    return new ResponseResult(returnCode, message, RequestTranscriptHelpers.ToHttpRequestMessage(Request));
        //}

        //protected internal virtual ResponseResult<T> Failure<T>(T content, string message = "")
        //{
        //    return new ResponseResult<T>(ReturnCodes.CODE_FAILURE, message, content, RequestTranscriptHelpers.ToHttpRequestMessage(Request));
        //}

        //#endregion

        #region OK Result => ReturnCode = 00
        protected internal virtual IActionResult Success(string message = "")
        {
            var baseResponse = new ApiReturnResult
            {
                Result = null,
                ReturnCode = ReturnCodes.CODE_SUCCESS,
                Message = message,
                ExceptionData = string.Empty
            };

            return Ok(baseResponse);
        }

        protected internal virtual IActionResult Success<T>(T content, string message = "")
        {
            var baseResponse = new ApiReturnResult
            {
                Result = content,
                ReturnCode = ReturnCodes.CODE_SUCCESS,
                Message = message,
                ExceptionData = string.Empty
            };

            return Ok(baseResponse);
        }
        #endregion

        #region Failure Result => ReturnCode = 99 or other
        protected internal virtual IActionResult Failure(string message = "")
        {
            var baseResponse = new ApiReturnResult
            {
                Result = null,
                ReturnCode = ReturnCodes.CODE_FAILURE,
                Message = message,
                ExceptionData = string.Empty
            };

            return Ok(baseResponse);
        }

        protected internal virtual IActionResult FailureCode(string returnCode, string message = "")
        {
            var baseResponse = new ApiReturnResult
            {
                Result = null,
                ReturnCode = returnCode,
                Message = message,
                ExceptionData = string.Empty
            };

            return Ok(baseResponse);
        }

        protected internal virtual IActionResult Failure<T>(T content, string message = "")
        {
            var baseResponse = new ApiReturnResult
            {
                Result = content,
                ReturnCode = ReturnCodes.CODE_FAILURE,
                Message = message,
                ExceptionData = string.Empty
            };

            return Ok(baseResponse);
        }
        #endregion
    }
}
