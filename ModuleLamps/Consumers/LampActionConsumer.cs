using Common;
using MassTransit;
using Model;
using ModuleLamps.API;
using ModuleLamps.Interfaces;

namespace ModuleLamps.Consumers
{
    internal class LampActionConsumer : IConsumer<ILampActionRequest>
    {
        private readonly ILampsStore _serviceLamps;
        public LampActionConsumer(ILampsStore servicePersons)
        {
            _serviceLamps = servicePersons;
        }

        public async Task Consume(ConsumeContext<ILampActionRequest> context)
        {
            var lamp = context.Message.Lamp;

            switch (context.Message.Action)
            {
                case CrudAction.Create:
                    await _serviceLamps.Add(lamp)
                        .ContinueWith(async t => await Respond(t));
                    
                    break;
                case CrudAction.Update:
                    await _serviceLamps.Update(lamp)
                        .ContinueWith(async t => await Respond(t));
                    break;
                case CrudAction.Delete:
                    await _serviceLamps.Delete(lamp)
                        .ContinueWith(async t => await Respond(t));
                    break;
            }

            async Task Respond(Task<ILamp> t)
            {
                var lamp = t.IsCompletedSuccessfully? 
                    t.Result : null;

                await context
                    .RespondAsync<ILampActionResponse>(new
                    {
                        RequestId = context.Message.RequestId,
                        Lamp = lamp,
                        IsSuccess = t.IsCompletedSuccessfully,
                        ErrorMessage = t.Exception?.GetJoinedError()
                    });
            }
        }
    }
}
