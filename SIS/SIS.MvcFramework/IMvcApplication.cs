using SIS.MvcFramework.DependencyContainer;

namespace SIS.MvcFramework
{
    public interface IMvcApplication
    {
        void Configure();

        void ConfigureServices(IServiceProvider serviceProvider);
    }
}