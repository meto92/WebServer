using SIS.Common;

namespace SIS.HTTP.Headers
{
    public class HttpHeader
    {
        public const string HostHeaderKey = "Host";
        public const string ContentType = "Content-Type";
        public const string ContentTypeText = "text/plain";
        public const string ContentTypeHtml = "text/html";
        public const string Location = "Location";
        public const string Cookie = "Cookie";
        public const string SetCookie = "Set-Cookie";
        public const string ContentLength = "Content-Length";
        public const string ContentDisposition = "Content-Disposition";
        public const string ContentApplicationJson = "application/json";
        public const string ContentApplicationXml = "application/xml";
        public const string ContentAttachment = "attachment";

        public HttpHeader(string key, string value)
        {
            key.ThrowIfNullOrEmpty(nameof(key));
            value.ThrowIfNullOrEmpty(nameof(value));

            this.Key = key;
            this.Value = value;
        }

        public string Key { get; }

        public string Value { get; }

        public override string ToString()
            => $"{this.Key}: {this.Value}";
    }
}