using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using SIS.Framework.Routing;

namespace SIS.Framework
{
    public class Server
    {
        private const string LocalHostIPAddress = "127.0.0.1";
        private const string ServerStartedMessage = "Server startet at http://{0}:{1}";

        private readonly int port;
        private readonly TcpListener listener;
        private readonly IServerRoutingTable serverRoutingTable;
        private bool isRunning;

        public Server(int port, IServerRoutingTable serverRoutingTable)
        {
            this.port = port;
            this.listener = new TcpListener(IPAddress.Parse(LocalHostIPAddress), port);

            this.serverRoutingTable = serverRoutingTable;
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
                this.serverRoutingTable);

            await connectionHandler.ProcessRequestAsync();
        }
    }
}