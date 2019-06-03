using SIS.HTTP.Enums;
using SIS.HTTP.Headers;

namespace SIS.MvcFramework.Results
{
    public class InlineResourceResult : ActionResult
    {
        public InlineResourceResult(
            byte[] content,
            HttpResponseStatusCode responseStatusCode = HttpResponseStatusCode.OK)
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