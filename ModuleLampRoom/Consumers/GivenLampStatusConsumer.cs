using Common;
using MassTransit;

namespace ModuleLampRoom.Consumers
{
    //internal class GivenLampStatusConsumer : IConsumer<IGivenLampStatusRequest>
    //{
    //    private readonly IServiceLampRoom _serviceLampRoom;
    //    public GivenLampStatusConsumer(IServiceLampRoom serviceLampRoom)
    //    {
    //        _serviceLampRoom = serviceLampRoom;
    //    }

    //    public async Task Consume(ConsumeContext<IGivenLampStatusRequest> context)
    //    {
    //        var msg = context.Message;
    //        await _serviceLampRoom.ChangeGivenLamp(msg.GivenPair, msg.Status);

    //        await context.RespondAsync<IGivenLampStatusResponse>(new { });
    //    }
    //}
}
