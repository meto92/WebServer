using System;
using System.Collections.Generic;
using System.Linq;

using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

namespace SIS.WebServer.Routing
{
    public class ServerRoutingTable : IServerRoutingTable
    {
        private readonly IDictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IHttpResponse>>> routes;

        public ServerRoutingTable()
        {
            this.routes = new Dictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IHttpResponse>>>();

            Enum.GetValues(typeof(HttpRequestMethod))
                .Cast<HttpRequestMethod>()
                .ToList()
                .ForEach(httpRequestMethod =>
                {
                    this.routes[httpRequestMethod] =
                        new Dictionary<string, Func<IHttpRequest, IHttpResponse>>();
                });
        }

        public void Get(string path, Func<IHttpRequest, IHttpResponse> func)
            => this.routes[HttpRequestMethod.Get][path] = func;

        public void Post(string path, Func<IHttpRequest, IHttpResponse> func)
            => this.routes[HttpRequestMethod.Post][path] = func;

        public void Put(string path, Func<IHttpRequest, IHttpResponse> func)
            => this.routes[HttpRequestMethod.Put][path] = func;

        public void Delete(string path, Func<IHttpRequest, IHttpResponse> func)
            => this.routes[HttpRequestMethod.Delete][path] = func;

        public void Add(HttpRequestMethod method, string path, Func<IHttpRequest, IHttpResponse> func)
        {
            throw new NotImplementedException();
        }

        public bool Contains(HttpRequestMethod requestMethod, string path)
            => this.routes[requestMethod].ContainsKey(path);

        public Func<IHttpRequest, IHttpResponse> Get(HttpRequestMethod requestMethod, string path)
        {
            // Don't do this
            return this.routes.TryGetValue(requestMethod, out Dictionary<string, Func<IHttpRequest, IHttpResponse>> funcs)
                ? funcs.TryGetValue(path, out Func<IHttpRequest, IHttpResponse> func)
                    ? func
                    : null
                : null;
        }
    }
}