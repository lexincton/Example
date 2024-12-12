using System.Linq.Dynamic.Core;
using Contracts;
using Contracts.V1;
using Grpc.Core;
using GRPCGateway.Helpers;
using MassTransit;
using Model;
using ModuleLamps.API;

namespace GRPCGateway.Services
{
    public class LampsServiceV1 : LampIntegration.LampIntegrationBase
    {
        private readonly ILogger<LampsServiceV1> _logger;
        private readonly IBus _bus;

        public LampsServiceV1(ILogger<LampsServiceV1> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public override async Task<Lamp> GetLampById(LongMessage request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var lamp = _bus
                    .Request<IGetLampByIdRequest, ILampActionResponse>(
                    new { request.Id })
                    .Result.Message.Lamp;

                return ConvertToMessage(lamp);
            });
        }

        public override async Task<Lamp> LampAdd(Lamp request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var response = _bus.Request<ILampActionRequest, ILampActionResponse>(
                new {
                    Action = Common.CrudAction.Create,
                    Lamp = ConvertFromMessage(request)
                }).Result.Message;

                return ConvertToMessage(response.Lamp);
            });
        }

        public override Task<EmptyMessage> LampDelete(Lamp request, ServerCallContext context)
        {
            return IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var response = _bus.Request<ILampActionRequest, ILampActionResponse>(new
                {
                    Action = Common.CrudAction.Delete,
                    Lamp = ConvertFromMessage(request)
                }).Result.Message;

                response.ThrowIfNotSuccess();

                return new EmptyMessage();
            });
        }

        public override async Task<Lamp> LampUpdate(Lamp request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var response = _bus.Request<ILampActionRequest, ILampActionResponse>(new
                {
                    Action = Common.CrudAction.Update,
                    Lamp = ConvertFromMessage(request)
                }).Result.Message;

                response.ThrowIfNotSuccess();

                return ConvertToMessage(response.Lamp);
            });
        }


        public override async Task<LampList> GetLamps(MessageString request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var response = _bus
                    .Request<IGetLampsRequest, IGetLampsResponse>(
                        new { request.Filter }
                    ).Result.Message;

                response.ThrowIfNotSuccess();

                var lampsMsg = new LampList();

                lampsMsg.Lamps
                    .AddRange(response.Lamps
                    .Select(x => ConvertToMessage(x)));

                return lampsMsg;
            });
        }

        internal static object ConvertFromMessage(Lamp lamp)
        {
            return new
            {
                lamp.Id,
                lamp.Name,
            };
        }

        internal static Lamp ConvertToMessage(ILamp lamp)
        {
            if (lamp == null)
                return new();

            return new Lamp
            {
                Id = lamp.Id,
                Name = lamp.Name,
            };
        }
    }

}
