using SIS.Framework.Routing;

namespace SIS.Framework
{
    public static class WebHost
    {
        public static void Start(IMvcApplication app)
        {
            IServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            app.ConfigureServices();
            app.Configure(serverRoutingTable);

            new Server(80, serverRoutingTable).Run();
        }
    }
}