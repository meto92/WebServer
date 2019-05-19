﻿using System;
using System.Net.Sockets;
using System.Text;

using SIS.HTTP.Common;
using SIS.HTTP.Enums;
using SIS.HTTP.Exceptions;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;
using SIS.WebServer.Routing;

namespace SIS.WebServer
{
    public class ConnectionHandler
    {
        private const string ActionNotFoundMessage = "Route with method {0} and path \"{1}\" not found";
        private const string ProcessingRequestMessage = "Processing: {0} {1} . . .";

        private readonly Socket client;
        private readonly IServerRoutingTable serverRoutingTable;

        public ConnectionHandler(
            Socket client, 
            IServerRoutingTable serverRoutingTable)
        {
            CoreValidator.ThrowIfNull(client, nameof(client));
            CoreValidator.ThrowIfNull(serverRoutingTable, nameof(serverRoutingTable));

            this.client = client;
            this.serverRoutingTable = serverRoutingTable;
        }

        private IHttpRequest ReadRequest()
        {
            StringBuilder result = new StringBuilder();
            ArraySegment<byte> data = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                int readBytesCount = this.client.Receive(data.Array, SocketFlags.None);

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

        private IHttpResponse HandleRequest(IHttpRequest httpRequest)
        {
            if (!this.serverRoutingTable.Contains(httpRequest.RequestMethod, httpRequest.Path))
            {
                return new TextResult(
                    string.Format(
                        ActionNotFoundMessage,
                        httpRequest.RequestMethod,
                        httpRequest.Path),
                    HttpResponseStatusCode.NotFound);
            }

            return this.serverRoutingTable.Get(httpRequest.RequestMethod, httpRequest.Path).Invoke(httpRequest);
        }        

        private void PrepareResponse(IHttpResponse httpResponse)
        {
            byte[] byteSegments = httpResponse.GetBytes();

            this.client.Send(byteSegments, SocketFlags.None);
        }

        private void ShutdownClient()
            => this.client.Shutdown(SocketShutdown.Both);

        public void ProcessRequest()
        {
            IHttpResponse httpResponse = null;

            try
            {
                IHttpRequest httpRequest = this.ReadRequest();

                if (httpRequest == null)
                {
                    this.ShutdownClient();

                    return;
                }

                Console.WriteLine(string.Format(
                    ProcessingRequestMessage,
                    httpRequest.RequestMethod,
                    httpRequest.Path));

                httpResponse = this.HandleRequest(httpRequest);

                Console.WriteLine();
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