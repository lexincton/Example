using Common;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ModulePersons.API;
using ModulePersons.Consumers;
using ModulePersons.Interfaces;
using ModulePersons.Objects;

namespace ModulePersons
{
    public class ModulePersons : IModule
    {
        public ModulePersons()
        {
        }

        public void RegisterServices(IServiceCollection services)
        {           
            services.AddSingleton<IServicePersonsMonolit, ServicePersonsMonolit>();
            services.AddSingleton<IPersonsStore, PersonsStore>();

            // Added for Microservices
            services.AddMassTransit(x =>
            {
                x.AddConsumer<GetPersonsConsumer>();
                x.AddConsumer<PersonActionConsumer>();
                x.AddConsumer<GetPersonByIdConsumer>();                
            });
        }
    }
}
