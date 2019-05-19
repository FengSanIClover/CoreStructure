using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TP5.Core.NetCore.Helper
{
    public class EnvWebHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EnvWebHelper(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// 取得用戶端 IP
        /// </summary>
        /// <returns>用戶端 IP</returns>
        public string GetClientIP()
        {
            //if (HttpContext.Current != null) return HttpContext.Current.Request.UserHostAddress;
            //else return EnvHelper.GetServerIP();
            if (this._httpContextAccessor == null) return EnvHelper.GetServerIP();
            return _httpContextAccessor.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            
        }

        /// <summary>
        /// 取得用戶端代理程式
        /// </summary>
        /// <returns>用戶端代理程式</returns>
        public string GetUserAgent()
        {
            //if (HttpContext.Current != null) return HttpContext.Current.Request.UserAgent;
            //else return string.Empty;
            if (this._httpContextAccessor == null) return string.Empty;
            return _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
        }
    }
}
