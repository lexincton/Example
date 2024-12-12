using Common;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ModuleLampRoom.Interfaces;
using ModuleLampRoom.Objects;
using ModuleLampRoom.Sagas;

namespace ModuleLampRoom
{
    public class ModuleLampRoom : IModule
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IServiceLampRoomMonolit, ServiceLampRoomMonolit>();
            services.AddSingleton<ILampRoomStore, LampRoomStore>();

            // Added for Microservices
            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<GivenLampSaga, GivenLampSagaState>();
                x.AddSagaStateMachine<GetGivenLampsSaga, GetGivenLampsState>();
            });
        }
    }


}
