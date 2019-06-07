using SIS.HTTP.Enums;
using SIS.HTTP.Responses;

namespace SIS.MvcFramework.Results
{
    public abstract class ActionResult : HttpResponse, IActionResult
    {
        protected ActionResult(HttpResponseStatusCode httpResponseStatusCode)
            : base(httpResponseStatusCode)
        { }
    }
}