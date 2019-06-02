using System.Text;

using SIS.HTTP.Common;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Responses;

namespace SIS.MvcFramework.Results
{
    public class HtmlResult : HttpResponse
    {
        public HtmlResult(
            string content,
            HttpResponseStatusCode responseStatusCode)
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