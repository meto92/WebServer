using System.Text;

using SIS.HTTP.Common;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Responses;

namespace SIS.Framework.Results
{
    public class TextResult : HttpResponse
    {
        public TextResult(
            string content,
            HttpResponseStatusCode responseStatusCode,
            string contentType = HttpHeader.ContentTypeText + "; " + GlobalConstants.CharsetUTF8)
            : base(responseStatusCode)
        {
            HttpHeader contentTypeHeaderr = new HttpHeader(
                HttpHeader.ContentType, contentType);
            
            this.Headers.AddHeader(contentTypeHeaderr);
            this.Content = Encoding.UTF8.GetBytes(content);
        }

        public TextResult(
            byte[] content,
            HttpResponseStatusCode responseStatusCode,
            string contentType = HttpHeader.ContentTypeText + "; " + GlobalConstants.CharsetUTF8)
            : base(responseStatusCode)
        {
            HttpHeader contentTypeHeaderr = new HttpHeader(
                HttpHeader.ContentType, contentType);

            this.Content = content;
            this.Headers.AddHeader(contentTypeHeaderr);
        }
    }
}