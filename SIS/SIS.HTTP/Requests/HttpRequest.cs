using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using SIS.Common;
using SIS.HTTP.Common;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Exceptions;
using SIS.HTTP.Headers;
using SIS.HTTP.Sessions;

namespace SIS.HTTP.Requests
{
    public class HttpRequest : IHttpRequest
    {
        public HttpRequest(string requestString)
        {
            requestString.ThrowIfNullOrEmpty(nameof(requestString));

            this.FormData = new Dictionary<string, ISet<string>>();
            this.QueryData = new Dictionary<string, ISet<string>>();
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();
            
            this.ParseRequest(requestString);
        }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public IDictionary<string, ISet<string>> FormData { get; }

        public IDictionary<string, ISet<string>> QueryData { get; }

        public IHttpHeaderCollection Headers { get; }

        public HttpRequestMethod RequestMethod { get; private set; }
      
        public IHttpCookieCollection Cookies { get; }

        public IHttpSession Session { get; set; }

        private bool IsValidRequestLine(string[] requestLineParameters)
            => requestLineParameters.Length == 3
                && requestLineParameters[2] == GlobalConstants.HttpOneProtocolFragment;

        private bool IsValidRequestQueryString()
            => this.Url.Contains("?")
                && this.Url.Split('?')[1]
                    .Contains("=");

        private void ParseRequestMethod(string[] requestLineParameters)
        {
            string requestMethodStr = requestLineParameters[0];

            if (!Enum.TryParse(requestMethodStr, true, out HttpRequestMethod requestMethod))
            {
                throw new BadRequestException(string.Format(
                    GlobalConstants.UnsupportedHttpRequestMethodMessage,
                    requestMethodStr));
            }

            this.RequestMethod = requestMethod;
        }

        private void ParseRequestUrl(string[] requestLine)
            => this.Url = requestLine[1];

        private void ParseRequestPath()
            => this.Path = this.Url.Split(
                new[] { '?', '#' },
                StringSplitOptions.None)
                [0];

        private void ParseHeaders(string[] requestContent)
        {
            int emptyLineIndex = Array.IndexOf(requestContent, string.Empty);

            for (int i = 0; i < emptyLineIndex; i++)
            {
                string[] kvp = requestContent[i].Split(new[] { ": "}, StringSplitOptions.None);

                string key = kvp[0];
                string value = kvp[1];

                HttpHeader header = new HttpHeader(key, value);

                this.Headers.AddHeader(header);
            }

            if (!this.Headers.ContainsHeader(HttpHeader.HostHeaderKey))
            {
                throw new BadRequestException();
            }
        }

        private void ParseCookies()
        {
            if (!this.Headers.ContainsHeader(HttpHeader.Cookie))
            {
                return;
            }

            string cookiesValue = this.Headers
                .GetHeader(HttpHeader.Cookie)
                .Value;

            string[] cookieParts = cookiesValue.Split(new[] { "; " }, StringSplitOptions.None);

            foreach (string[] cookieKvp in cookieParts
                .Select(kvp => kvp.Split('='))
                .Where(kvp => kvp.Length == 2))
            {
                (string key, string value) = (cookieKvp[0], cookieKvp[1]);

                HttpCookie cookie = new HttpCookie(key, value, false);

                this.Cookies.AddCookie(cookie);
            }
        }

        private void SaveParameters(string[] parameters, IDictionary<string, ISet<string>> dict)
        {
            foreach (string[] kvp in parameters
                .Select(kvp => kvp.Split('=')))
            {
                if (kvp.Length != 2)
                {
                    throw new BadRequestException();
                }

                string key = kvp[0];
                string value = WebUtility.UrlDecode(kvp[1]);

                if (!dict.ContainsKey(key))
                {
                    dict[key] = new HashSet<string>();
                }

                dict[key].Add(value);
            }
        }

        private void ParseQueryParameters()
        {
            if (!this.IsValidRequestQueryString())
            {
                return;
            }

            string[] queryParameters = this.Url.Split(new[] { '?', '#' })[1]
                .Split('&');

            this.SaveParameters(queryParameters, this.QueryData);
        }

        private void ParseFormDataParameters(string formData)
        {
            if (formData == string.Empty)
            {
                return;
            }

            string[] formDataParameters = formData.Split('&');

            this.SaveParameters(formDataParameters, this.FormData);            
        }

        private void ParseRequestParameters(string formData)
        {
            this.ParseQueryParameters();
            this.ParseFormDataParameters(formData);
        }

        private void ParseRequest(string requestString)
        {
            string[] splitRequestContent = requestString.Split(new[] { GlobalConstants.HttpNewLine }, StringSplitOptions.None);

            string[] requestLineParameters = splitRequestContent[0].Trim()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (!this.IsValidRequestLine(requestLineParameters))
            {
                throw new BadRequestException();
            }

            this.ParseRequestMethod(requestLineParameters);
            this.ParseRequestUrl(requestLineParameters);
            this.ParseRequestPath();

            this.ParseHeaders(splitRequestContent.Skip(1).ToArray());
            this.ParseCookies();

            this.ParseRequestParameters(splitRequestContent.Last());
        }
    }
}