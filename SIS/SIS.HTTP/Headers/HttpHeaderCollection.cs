﻿using System.Collections.Generic;

using SIS.Common;
using SIS.HTTP.Common;

namespace SIS.HTTP.Headers
{
    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private readonly IDictionary<string, HttpHeader> headers;

        public HttpHeaderCollection()
            => this.headers = new Dictionary<string, HttpHeader>();

        public void AddHeader(HttpHeader header)
        {
            header.ThrowIfNull(nameof(header));

            this.headers[header.Key] = header;
        }

        public bool ContainsHeader(string key)
        {
            key.ThrowIfNullOrEmpty(nameof(key));

            return this.headers.ContainsKey(key);
        }

        public HttpHeader GetHeader(string key)
        {
            key.ThrowIfNullOrEmpty(nameof(key));

            return this.headers.TryGetValue(key, out HttpHeader header)
                ? header
                : null;
        }

        public override string ToString()
            => string.Join(
                GlobalConstants.HttpNewLine,
                this.headers.Values);
    }
}