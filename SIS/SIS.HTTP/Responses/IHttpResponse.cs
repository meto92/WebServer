using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;

namespace SIS.HTTP.Responses
{
    public interface IHttpResponse
    {
        HttpResponseStatusCode StatusCode { get; set; }

        IHttpHeaderCollection Headers { get; }

        void AddCookie(HttpCookie cookie);

        byte[] Content { get; set; }

        void AddHeader(HttpHeader header);

        byte[] GetBytes();
    }
}