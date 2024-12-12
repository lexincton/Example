using Contracts;
using Contracts.V2;
using Grpc.Core;
using GRPCGateway.Helpers;
using ModuleLampRoom;
using ModuleLampRoom.API;
using GivenLampStatus = ModuleLampRoom.API.GivenLampStatus;

namespace GRPCGateway.Services
{
    public class GivenLampServiceV2 : GivenLampIntegration.GivenLampIntegrationBase
    {
        private readonly ILogger<GivenLampServiceV2> _logger;
        private readonly IServiceLampRoomMonolit _serviceLampRoom;

        public GivenLampServiceV2(
            ILogger<GivenLampServiceV2> logger,
            IServiceLampRoomMonolit serviceLampRoom)
        {
            _logger = logger;
            _serviceLampRoom = serviceLampRoom;
        }

        public override async Task<EmptyMessage> ChangeGivenLamp(GivenLampRequest request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                _serviceLampRoom.ChangeGivenLamp(
                    IGivenPair
                        .Create(request.GivenLamp.Person.Id, request.GivenLamp.Lamp.Id),
                    (GivenLampStatus)request.GivenLampAction).GetAwaiter().GetResult();

                return new EmptyMessage();
            });
        }

        public override async Task<GivenLampList> GetGivenLamps(EmptyMessage request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var response = _serviceLampRoom.GetAllGivenLamps().Result;

                var givensList = new GivenLampList();

                givensList.GivenLamps
                    .AddRange(response
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
