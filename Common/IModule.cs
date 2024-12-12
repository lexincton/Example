using Microsoft.Extensions.DependencyInjection;

namespace Common
{
    public interface IModule
    {
        void RegisterServices(IServiceCollection services);
    }
}
