using System.Text;

using SIS.HTTP.Enums;
using SIS.HTTP.Headers;

namespace SIS.MvcFramework.Results
{
    public class XmlResult : ActionResult
    {
        public XmlResult(string xmlContent, HttpResponseStatusCode httpResponseStatusCode = HttpResponseStatusCode.OK)
            : base(httpResponseStatusCode)
        {
            this.Content = Encoding.UTF8.GetBytes(xmlContent);
            this.AddHeader(HttpHeader.ContentType, HttpHeader.ContentApplicationXml);
        }
    }
}