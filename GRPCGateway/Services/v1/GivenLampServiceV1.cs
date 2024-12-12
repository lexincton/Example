using Contracts;
using Contracts.V1;
using Grpc.Core;
using GRPCGateway.Helpers;
using MassTransit;
using ModuleLampRoom.API;
using GivenLampStatus = ModuleLampRoom.API.GivenLampStatus;

namespace GRPCGateway.Services
{
    public class GivenLampServiceV1 : GivenLampIntegration.GivenLampIntegrationBase
    {
        private readonly ILogger<GivenLampServiceV1> _logger;
        private readonly IBus _bus;

        public GivenLampServiceV1(ILogger<GivenLampServiceV1> logger, IBus bus)
        {
            _bus = bus;
            _logger = logger;
        }

        public override async Task<EmptyMessage> ChangeGivenLamp(GivenLampRequest request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var response = _bus.Request<IGivenLampRequest, IGivenLampResponse>(new
                {
                    GivenPair = IGivenPair
                        .Create(request.GivenLamp.Person.Id, request.GivenLamp.Lamp.Id),
                    Status = (GivenLampStatus)request.GivenLampAction
                }).Result.Message;

                response.ThrowIfNotSuccess();

                return new EmptyMessage();
            });
        }

        public override async Task<GivenLampList> GetGivenLamps(EmptyMessage request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var response = _bus.Request<IGetGivenLampsRequest, IGetGivenLampsResponse>(new
                {
                }).Result.Message;

                response.ThrowIfNotSuccess();

                var givensList = new GivenLampList();

                givensList.GivenLamps
                    .AddRange(response.Givens
                        .Select(x => new GivenLampData()
                        {
                            Person = PersonsServiceV2.ConvertToMessage(x.Person),
                            Lamp = LampsServiceV2.ConvertToMessage(x.Lamp)
                        }));

                return givensList;
            });
        }
    }
}
