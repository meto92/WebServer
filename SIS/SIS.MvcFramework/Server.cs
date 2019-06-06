using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using SIS.Common;
using SIS.MvcFramework.Routing;
using SIS.MvcFramework.Sessions;

namespace SIS.MvcFramework
{
    public class Server
    {
        private const string LocalHostIPAddress = "127.0.0.1";
        private const string ServerStartedMessage = "Server startet at http://{0}:{1}";

        private readonly int port;
        private readonly TcpListener listener;
        private readonly IServerRoutingTable serverRoutingTable;
        private readonly IHttpSessionStorage httpSessionStorage;
        private bool isRunning;

        public Server(int port, IServerRoutingTable serverRoutingTable, IHttpSessionStorage httpSessionStorage)
        {
            serverRoutingTable.ThrowIfNull(nameof(serverRoutingTable));
            httpSessionStorage.ThrowIfNull(nameof(httpSessionStorage));
            
            this.port = port;
            this.listener = new TcpListener(IPAddress.Parse(LocalHostIPAddress), port);

            this.serverRoutingTable = serverRoutingTable;
            this.httpSessionStorage = httpSessionStorage;
        }

        public void Run()
        {
            this.listener.Start();
            this.isRunning = true;

            Console.WriteLine(string.Format(
                ServerStartedMessage,
                LocalHostIPAddress,
                this.port));

            while (this.isRunning)
            {
                Socket client = this.listener.AcceptSocket();

                Task.Run(() => this.Listen(client));
            }
        }

        public async Task Listen(Socket client)
        {
            ConnectionHandler connectionHandler = new ConnectionHandler(
                client, 
                this.serverRoutingTable,
                this.httpSessionStorage);

            await connectionHandler.ProcessRequestAsync();
        }
    }
}