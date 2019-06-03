using System.Text;

using SIS.HTTP.Enums;
using SIS.HTTP.Headers;

namespace SIS.MvcFramework.Results
{
    public class JsonResult : ActionResult
    {
        public JsonResult(string jsonContent, HttpResponseStatusCode httpResponseStatusCode = HttpResponseStatusCode.OK)
            : base(httpResponseStatusCode)
        {
            this.Content = Encoding.UTF8.GetBytes(jsonContent);
            this.AddHeader(HttpHeader.ContentType, HttpHeader.ContentApplicationJson);
        }
    }
}