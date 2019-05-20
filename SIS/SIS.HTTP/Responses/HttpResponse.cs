using System;
using System.Linq;
using System.Text;

using SIS.HTTP.Common;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Extensions;
using SIS.HTTP.Headers;

namespace SIS.HTTP.Responses
{
    public class HttpResponse : IHttpResponse
    {
        public HttpResponse()
        {
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();
            this.Content = new byte[0];
        }

        public HttpResponse(HttpResponseStatusCode statusCode)
            : this()
            => this.StatusCode = statusCode;

        public HttpResponseStatusCode StatusCode { get ; set ; }

        public IHttpHeaderCollection Headers { get; }

        public IHttpCookieCollection Cookies { get; }

        public void AddCookie(HttpCookie cookie)
            => this.Cookies.AddCookie(cookie);

        public byte[] Content { get; set; }

        public void AddHeader(HttpHeader header) 
            => this.Headers.AddHeader(header);

        public byte[] GetBytes()
        {
            byte[] toStringBytes = Encoding.UTF8.GetBytes(this.ToString());

            byte[] result = new byte[toStringBytes.Length + this.Content.Length];

            Array.Copy(toStringBytes, 0, result, 0, toStringBytes.Length);
            Array.Copy(this.Content, 0, result, toStringBytes.Length, this.Content.Length);
            
            return result;

            //return Encoding.UTF8.GetBytes(this.ToString())
            //    .Concat(this.Content)
            //    .ToArray();
        }

        public override string ToString()
        {
            StringBuilder cookieHeaders = new StringBuilder();

            this.Cookies
                .ToList()
                .ForEach(cookie =>
                {
                    HttpHeader cookieHeader = new HttpHeader(
                        HttpHeader.SetCookie, 
                        cookie.ToString());

                    cookieHeaders.Append(cookieHeader.ToString())
                        .Append(GlobalConstants.HttpNewLine);
                });

            string result = string.Concat(
                $"{GlobalConstants.HttpOneProtocolFragment} {this.StatusCode.GetResponseLine()}",
                GlobalConstants.HttpNewLine,
                this.Headers,
                GlobalConstants.HttpNewLine,
                cookieHeaders.ToString(),
                GlobalConstants.HttpNewLine);
            
            return result;
        }
    }
}