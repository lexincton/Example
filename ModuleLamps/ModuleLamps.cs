using Common;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ModuleLamps.Consumers;
using ModuleLamps.Interfaces;
using ModuleLamps.Objects;

namespace ModuleLamps
{
    public class ModuleLamps : IModule
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IServiceLampsMonolit, ServiceLampsMonolit>();
            services.AddSingleton<ILampsStore, LampsStore>();

            // Added for Microservices
            services.AddMassTransit(x =>
            {
                x.AddConsumer<GetLampsConsumer>();
                x.AddConsumer<LampActionConsumer>();
                x.AddConsumer<GetLampByIdConsumer>();
            });
        }
    }
}
