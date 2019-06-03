using SIS.HTTP.Enums;
using SIS.HTTP.Headers;

namespace SIS.MvcFramework.Results
{
    public class FileResult : ActionResult
    {
        public FileResult(byte[] fileContent, HttpResponseStatusCode httpResponseStatusCode = HttpResponseStatusCode.OK) 
            : base(httpResponseStatusCode)
        {
            this.Content = fileContent;
            this.AddHeader(HttpHeader.ContentLength, fileContent.Length.ToString());
            this.AddHeader(HttpHeader.ContentDisposition, HttpHeader.ContentAttachment);
        }
    }
}