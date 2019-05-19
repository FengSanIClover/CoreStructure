
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace TP5.Core.NetCore
{
    public class CoreWeb
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _userId;
        public static string IdentityName;

        public CoreWeb(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IdentityName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
        }
        //public static string IdentityName
        //{
        //    get
        //    {
        //        var clamPrincipal = HttpContext.Current.User as ClaimsPrincipal;                
        //        return clamPrincipal.Identity.IsAuthenticated ? clamPrincipal.Identity.Name : "NotLoginForTest";                                
        //    }
        //}


    }
}
