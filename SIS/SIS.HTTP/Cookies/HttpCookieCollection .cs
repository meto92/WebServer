using System.Collections;
using System.Collections.Generic;

using SIS.HTTP.Common;

namespace SIS.HTTP.Cookies
{
    public class HttpCookieCollection : IHttpCookieCollection
    {
        private IDictionary<string, HttpCookie> cookies;

        public HttpCookieCollection()
        {
            this.cookies = new Dictionary<string, HttpCookie>();
        }

        public void AddCookie(HttpCookie cookie)
        {
            CoreValidator.ThrowIfNull(cookie, nameof(cookie));

            this.cookies[cookie.Key] = cookie;
        }

        public bool ContainsCookie(string key)
            => this.cookies.ContainsKey(key);

        public HttpCookie GetCookie(string key)
            => this.cookies.TryGetValue(key, out HttpCookie cookie)
                ? cookie
                : null;

        public bool HasCookies() => this.cookies.Count != 0;

        public IEnumerator<HttpCookie> GetEnumerator()
            => this.cookies.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
