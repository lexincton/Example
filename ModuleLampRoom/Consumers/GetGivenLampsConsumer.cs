using Common;
using MassTransit;
using ModuleLampRoom.API;

namespace ModuleLampRoom.Consumers
{
    //internal class GetGivenLampsConsumer : IConsumer<IGetGivenLampsRequest>
    //{
    //    private readonly IServiceLampRoom _serviceLampRoom;
    //    public GetGivenLampsConsumer(IServiceLampRoom serviceLampRoom)
    //    {
    //        _serviceLampRoom = serviceLampRoom;
    //    }

    //    public async Task Consume(ConsumeContext<IGetGivenLampsRequest> context)
    //    {
    //        await _serviceLampRoom.GetAllGivenLamps()
    //            .ContinueWith(async t =>
    //            {
    //                var givens = t.IsCompletedSuccessfully ? 
    //                    t.Result : Enumerable.Empty<IGivenPair>();

    //                await context
    //                    .RespondAsync<IGetGivenLampsResponse>(new
    //                    {
    //                        RequestId = context.Message.RequestId,
    //                        Givens = givens,
    //                        IsSuccess = t.IsCompletedSuccessfully,
    //                        ErrorMessage = t.Exception?.GetJoinedError()
    //                    });
    //            });
    //    }
    //}
}
