using SIS.HTTP.Common;

namespace SIS.HTTP.Headers
{
    public class HttpHeader
    {
        public const string HostHeaderKey = "Host";
        public const string ContentType = "Content-Type";
        public const string ContentTypeText = "text/plain";
        public const string ContentTypeHtml = "text/html";
        public const string Location = "Location";

        public HttpHeader(string key, string value)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));
            CoreValidator.ThrowIfNullOrEmpty(value, nameof(value));

            this.Key = key;
            this.Value = value;
        }

        public string Key { get; }

        public string Value { get; }

        public override string ToString()
            => $"{this.Key}: {this.Value}";
    }
}