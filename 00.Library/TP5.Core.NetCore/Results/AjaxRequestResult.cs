
namespace TP5.Core.NetCore.Results
{
    public class AjaxRequestResult
    {
        public AjaxRequestResult(string msg = "", bool success = true)
        {
            this.Success = success;
            this.Msg = msg;
        }

        public bool Success { get; set; }

        public string Msg { get; set; }

        public object Obj { get; set; }
    }
}
