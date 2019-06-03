using SIS.HTTP.Enums;
using SIS.HTTP.Headers;

namespace SIS.MvcFramework.Results
{
    public class RedirectResult : ActionResult
    {
        public RedirectResult(string location)
            : base(HttpResponseStatusCode.SeeOther)
        {
            HttpHeader locationHeader = new HttpHeader(HttpHeader.Location, location);

            this.Headers.AddHeader(locationHeader);
        }
    }
}