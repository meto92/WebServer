using SIS.Framework.Routing;

namespace SIS.Framework
{
    public interface IMvcApplication
    {
        void Configure(IServerRoutingTable serverRoutingTable);

        void ConfigureServices();
    }
}