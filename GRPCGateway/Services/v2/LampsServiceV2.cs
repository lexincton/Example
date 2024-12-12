using System.Linq.Dynamic.Core;
using Common;
using Contracts;
using Contracts.V2;
using Grpc.Core;
using GRPCGateway.Helpers;
using Model;
using ModuleLamps.Objects;

namespace GRPCGateway.Services
{
    public class LampsServiceV2 : LampIntegration.LampIntegrationBase
    {
        private readonly ILogger<LampsServiceV2> _logger;
        private readonly IServiceLampsMonolit _serviceLamps;

        public LampsServiceV2(
            ILogger<LampsServiceV2> logger,
            IServiceLampsMonolit serviceLamps)
        {
            _logger = logger;
            _serviceLamps = serviceLamps;
        }

        public override async Task<Lamp> GetLampById(LongMessage request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var lamp = _serviceLamps.GetById(request.Id).Result;

                return ConvertToMessage(lamp);
            });
        }

        public override async Task<Lamp> LampAdd(Lamp request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var lamp = _serviceLamps.Add(ConvertFromMessage(request)).Result;

                return ConvertToMessage(lamp);
            });
        }

        public override Task<EmptyMessage> LampDelete(Lamp request, ServerCallContext context)
        {
            return IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var lamp = _serviceLamps.Delete(ConvertFromMessage(request)).Result;

                return new EmptyMessage();
            });
        }

        public override async Task<Lamp> LampUpdate(Lamp request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var lamp = _serviceLamps.Update(ConvertFromMessage(request)).Result;

                return ConvertToMessage(lamp);
            });
        }


        public override async Task<LampList> GetLamps(MessageString request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                if (!Helper.TryConvertStringToExpression<ILamp>(request.Filter, out var filterLamp))
                {
                    throw new Exception("Ошибка синтаксиса фильтра");
                }

                var lamps = _serviceLamps.GetAll(filterLamp).Result;
                var response = new LampList();

                response.Lamps
                    .AddRange(lamps
                    .Select(x => ConvertToMessage(x)));

                return response;
            });
        }

        internal static ILamp ConvertFromMessage(Lamp lamp)
        {
            var lampConvert = ILamp.Create();
            lampConvert.Id = lamp.Id;
            lampConvert.Name = lamp.Name;
            
            return lampConvert;
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
