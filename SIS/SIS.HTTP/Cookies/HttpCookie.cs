using System;
using SIS.HTTP.Common;

namespace SIS.HTTP.Cookies
{
    public class HttpCookie
    {
        private const int DefaultExpirationDay = 3;
        private const string DefaultPath = "/";

        public HttpCookie(
            string key,
            string value,
            int expires = DefaultExpirationDay,
            string path = DefaultPath,
            bool httpOnly = true)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));
            CoreValidator.ThrowIfNullOrEmpty(value, nameof(value));

            this.Key = key;
            this.Value = value;
            this.IsNew = true;
            this.Expires = DateTime.UtcNow.AddDays(expires);
            this.Path = path;
            this.HttpOnly = httpOnly;
        }

        public HttpCookie(
            string key,
            string value,
            bool isNew,
            int expires = DefaultExpirationDay,
            string path = DefaultPath)
            : this(key, value, expires, path)
            => this.IsNew = isNew;

        public string Key { get; }

        public string Value { get; }

        public DateTime Expires { get; private set; }

        public string Path { get; }

        public bool IsNew { get; }

        public bool HttpOnly { get; }

        public void Delete()
            => this.Expires = DateTime.UtcNow.AddDays(-1);

        public override string ToString()
        {
            string result = $"{this.Key}={this.Value}; Expires={this.Expires:R}; Path={this.Path}";

            if (this.HttpOnly)
            {
                result += "; HttpOnly";
            }

            return result;
        }
    }
}