using System.Collections.Generic;

using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Sessions;

namespace SIS.HTTP.Requests
{
    public interface IHttpRequest
    {
        string Path { get; }

        string Url { get; }

        IDictionary<string, object> FormData { get; }

        IDictionary<string, object> QueryData { get; }

        IHttpHeaderCollection Headers { get; }

        HttpRequestMethod RequestMethod { get; }

        IHttpCookieCollection Cookies { get; }

        IHttpSession Session { get; set; }
    }
}