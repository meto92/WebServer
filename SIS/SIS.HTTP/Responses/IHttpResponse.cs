using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;

namespace SIS.HTTP.Responses
{
    public interface IHttpResponse
    {
        HttpResponseStatusCode StatusCode { get; set; }

        IHttpHeaderCollection Headers { get; }

        IHttpCookieCollection Cookies { get; }

        void AddHeader(HttpHeader header);

        void AddCookie(HttpCookie cookie);

        byte[] Content { get; set; }

        byte[] GetBytes();
    }
}