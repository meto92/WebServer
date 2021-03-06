﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using SIS.Common;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Exceptions;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.MvcFramework.Results;
using SIS.MvcFramework.Routing;
using SIS.MvcFramework.Sessions;

namespace SIS.MvcFramework
{
    public class ConnectionHandler
    {
        private const string ActionNotFoundMessage = "Route with method {0} and path \"{1}\" not found";
        private const string ProcessingRequestMessage = "Processing: {0} {1} . . .";

        private readonly Socket client;
        private readonly IServerRoutingTable serverRoutingTable;
        private readonly IHttpSessionStorage httpSessionStorage;

        public ConnectionHandler(
            Socket client, 
            IServerRoutingTable serverRoutingTable,
            IHttpSessionStorage httpSessionStorage)
        {
            client.ThrowIfNull(nameof(client));
            serverRoutingTable.ThrowIfNull(nameof(serverRoutingTable));
            httpSessionStorage.ThrowIfNull(nameof(httpSessionStorage));

            this.client = client;
            this.serverRoutingTable = serverRoutingTable;
            this.httpSessionStorage = httpSessionStorage;
        }

        private async Task<IHttpRequest> ReadRequest()
        {
            StringBuilder result = new StringBuilder();
            ArraySegment<byte> data = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                int readBytesCount = await this.client.ReceiveAsync(data, SocketFlags.None);

                if (readBytesCount == 0)
                {
                    break;
                }

                string bytesAsString = Encoding.UTF8.GetString(data.Array, 0, readBytesCount);

                result.Append(bytesAsString);

                if (readBytesCount < 1024)
                {
                    break;
                }
            }

            return result.Length == 0
                ? null
                : new HttpRequest(result.ToString());
        }

        private IHttpResponse ReturnIfResource(string httpRequestPath)
        {
            string filePath = "../../../" + httpRequestPath;

            if (!File.Exists(filePath))
            {
                return null;
            }

            byte[] content = File.ReadAllBytes(filePath);

            IHttpResponse response = new InlineResourceResult(content);

            return response;
        }

        private IHttpResponse HandleRequest(IHttpRequest httpRequest)
        {
            if (this.serverRoutingTable.Contains(httpRequest.RequestMethod, httpRequest.Path))
            {
                Console.WriteLine(string.Format(
                    ProcessingRequestMessage,
                    httpRequest.RequestMethod,
                    httpRequest.Path));

                return this.serverRoutingTable
                    .Get(httpRequest.RequestMethod, httpRequest.Path)
                    .Invoke(httpRequest);
            }

            IHttpResponse response = this.ReturnIfResource(httpRequest.Path)
                ?? new TextResult(
                    string.Format(
                        ActionNotFoundMessage,
                        httpRequest.RequestMethod,
                        httpRequest.Path),
                    HttpResponseStatusCode.NotFound);

            return response;
        }

        private void PrepareResponse(IHttpResponse httpResponse)
        {
            byte[] byteSegments = httpResponse.GetBytes();

            this.client.Send(byteSegments, SocketFlags.None);
        }

        private void ShutdownClient()
            => this.client.Shutdown(SocketShutdown.Both);

        private string SetRequestSession(IHttpRequest httpRequest)
        {
            string sessionId = httpRequest.Cookies.ContainsCookie(HttpSessionStorage.SessionCookieKey)
                ? httpRequest.Cookies
                    .GetCookie(HttpSessionStorage.SessionCookieKey)
                    .Value
                : Guid.NewGuid().ToString();

            httpRequest.Session = this.httpSessionStorage.GetSession(sessionId);

            return httpRequest.Session.Id;
        }

        public async Task ProcessRequestAsync()
        {
            IHttpResponse httpResponse = null;

            try
            {
                IHttpRequest httpRequest = await this.ReadRequest();

                if (httpRequest == null)
                {
                    this.ShutdownClient();

                    return;
                }

                string sessionId = SetRequestSession(httpRequest);

                httpResponse = this.HandleRequest(httpRequest);

                httpResponse.AddCookie(new HttpCookie(
                    HttpSessionStorage.SessionCookieKey, 
                    sessionId));
            }
            catch (BadRequestException e)
            {
                httpResponse = new TextResult(e.ToString(), HttpResponseStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                httpResponse = new TextResult(e.ToString(), HttpResponseStatusCode.InternalServerError);
            }

            this.PrepareResponse(httpResponse);
            this.ShutdownClient();
        }
    }
}