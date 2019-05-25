using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Responses;

namespace SIS.WebServer.Results
{
    public class InlineResourceResult : HttpResponse
    {
        public InlineResourceResult(
            byte[] content,
            HttpResponseStatusCode responseStatusCode)
            : base(responseStatusCode)
        {
            HttpHeader contentLengthHeader = new HttpHeader(HttpHeader.ContentLength, content.Length.ToString());
            HttpHeader contentDispositionHeader = new HttpHeader(HttpHeader.ContentDisposition, "inline");

            base.Headers.AddHeader(contentLengthHeader);
            base.Headers.AddHeader(contentDispositionHeader);
            base.Content = content;
        }
    }
}