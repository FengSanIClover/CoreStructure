namespace TP5.Core.NetCore.Controllers
{
    [Authorize]
    [MvcExceptionHandler]
    public abstract class BaseMvcController : Controller
    {
        // 覆寫 JsonResult 改為採用 Json.Net 來做序列化
        protected override JsonResult Json(object data, string contentType,
                  Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            if (behavior == JsonRequestBehavior.DenyGet
                && string.Equals(this.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return new JsonResult(); // Call JsonResult to throw the same exception as JsonResult

            return new JsonNetResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }

        // REF: http://james.newtonking.com/archive/2008/10/16/asp-net-mvc-and-json-net.aspx
        public class JsonNetResult : JsonResult
        {
            public JsonSerializerSettings SerializerSettings { get; set; }

            public Formatting Formatting { get; set; }

            public JsonNetResult()
            {
                SerializerSettings = new JsonSerializerSettings();
                SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                var response = context.HttpContext.Response;
                response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

                if (ContentEncoding != null)
                    response.ContentEncoding = ContentEncoding;

                if (Data != null)
                {
                    JsonTextWriter writer = new JsonTextWriter(response.Output)
                    {
                        Formatting = Formatting
                    };

                    SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                    serializer.Serialize(writer, Data); writer.Flush();
                }
            }
        }
    }
}
