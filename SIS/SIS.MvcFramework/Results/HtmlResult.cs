using System.Text;

using SIS.HTTP.Common;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;

namespace SIS.MvcFramework.Results
{
    public class HtmlResult : ActionResult
    {
        public HtmlResult(
            string content,
            HttpResponseStatusCode responseStatusCode = HttpResponseStatusCode.OK)
            : base(responseStatusCode)
        {
            HttpHeader contentTypeHeaderr =
                new HttpHeader(
                    HttpHeader.ContentType,
                    HttpHeader.ContentTypeHtml + "; " + GlobalConstants.CharsetUTF8);

            this.Headers.AddHeader(contentTypeHeaderr);
            this.Content = Encoding.UTF8.GetBytes(content);
        }
    }
}