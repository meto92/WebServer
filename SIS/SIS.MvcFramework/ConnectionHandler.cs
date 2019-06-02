using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using SIS.HTTP.Common;
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

        public ConnectionHandler(
            Socket client, 
            IServerRoutingTable serverRoutingTable)
        {
            CoreValidator.ThrowIfNull(client, nameof(client));
            CoreValidator.ThrowIfNull(serverRoutingTable, nameof(serverRoutingTable));

            this.client = client;
            this.serverRoutingTable = serverRoutingTable;
        }

        private async Task<IHttpRequest> ReadRequest()
        {
            StringBuilder result = new StringBuilder();
            ArraySegment<byte> data = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                int readBytesCount = await this.client.ReceiveAsync(data.Array, SocketFlags.None);

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

            IHttpResponse response = new InlineResourceResult(content, HttpResponseStatusCode.OK);

            return response;
        }

        private IHttpResponse HandleRequest(IHttpRequest httpRequest)
        {
            if (this.serverRoutingTable.Contains(httpRequest.RequestMethod, httpRequest.Path))
            {
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

        private async Task PrepareResponse(IHttpResponse httpResponse)
        {
            byte[] byteSegments = httpResponse.GetBytes();

            await this.client.SendAsync(byteSegments, SocketFlags.None);
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

            httpRequest.Session = HttpSessionStorage.GetSession(sessionId);

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

                Console.WriteLine(string.Format(
                    ProcessingRequestMessage,
                    httpRequest.RequestMethod,
                    httpRequest.Path));

                string sessionId = SetRequestSession(httpRequest);

                httpResponse = this.HandleRequest(httpRequest);

                httpResponse.AddCookie(new HttpCookie(
                    HttpSessionStorage.SessionCookieKey, 
                    sessionId));

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

            await this.PrepareResponse(httpResponse);
            this.ShutdownClient();
        }
    }
}