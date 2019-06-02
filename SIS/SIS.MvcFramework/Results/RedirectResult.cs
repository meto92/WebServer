using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Responses;

namespace SIS.MvcFramework.Results
{
    public class RedirectResult : HttpResponse
    {
        public RedirectResult(string location)
            : base(HttpResponseStatusCode.SeeOther)
        {
            HttpHeader locationHeader = new HttpHeader(HttpHeader.Location, location);

            this.Headers.AddHeader(locationHeader);
        }
    }
}