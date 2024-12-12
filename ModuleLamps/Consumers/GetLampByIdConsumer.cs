using Common;
using MassTransit;
using ModuleLamps.API;
using ModuleLamps.Interfaces;

namespace ModuleLamps.Consumers
{
    internal class GetLampByIdConsumer : IConsumer<IGetLampByIdRequest>
    {
        private readonly ILampsStore _serviceLamps;
        public GetLampByIdConsumer(ILampsStore servicePersons)
        {
            _serviceLamps = servicePersons;
        }

        public async Task Consume(ConsumeContext<IGetLampByIdRequest> context)
        {
            await _serviceLamps.GetById(context.Message.Id)
                .ContinueWith(async t =>
                {
                    var lamp = t.IsCompletedSuccessfully ? t.Result : null;

                    await context
                        .RespondAsync<IGetLampByIdResponse>(new
                        {
                            RequestId = context.Message.RequestId,
                            Lamp = lamp,
                            IsSuccess = t.IsCompletedSuccessfully,
                            ErrorMessage = t.Exception?.GetJoinedError()
                        });
                });
        }
    }
}
