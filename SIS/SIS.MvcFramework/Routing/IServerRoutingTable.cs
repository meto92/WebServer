using System;

using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

namespace SIS.MvcFramework.Routing
{
    public interface IServerRoutingTable
    {
        void Add(HttpRequestMethod method, string path, Func<IHttpRequest, IHttpResponse> func);

        void Get(string path, Func<IHttpRequest, IHttpResponse> func);

        void Post(string path, Func<IHttpRequest, IHttpResponse> func);

        void Put(string path, Func<IHttpRequest, IHttpResponse> func);

        void Delete(string path, Func<IHttpRequest, IHttpResponse> func);

        bool Contains(HttpRequestMethod requestMethod, string path);

        Func<IHttpRequest, IHttpResponse> Get(HttpRequestMethod requestMethod, string path);
    }
}