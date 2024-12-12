using Common;
using MassTransit;
using Model;
using ModuleLamps.API;
using ModuleLamps.Interfaces;

namespace ModuleLamps.Consumers
{
    internal class GetLampsConsumer : IConsumer<IGetLampsRequest>
    {
        private readonly ILampsStore _serviceLamps;
        public GetLampsConsumer(ILampsStore servicePersons)
        {
            _serviceLamps = servicePersons;
        }

        public async Task Consume(ConsumeContext<IGetLampsRequest> context)
        {
            //var tt = new long[] { 1 };
            //Expression<Func<IDevice, bool>> t = x=> new long[] { 1 }.Contains(x.Id);

            ////var vis = new ExpressionToStringVisitor();
            ////var s = vis.Convert(t);


            //Helper.TryConvertExpressionToString(t, out var str);

            ////str = "new System.Int64[] { 1 }.Contains(x.Id)";

            //Helper.TryConvertStringToExpression<IDevice>(str, out var expr);

            Helper.TryConvertStringToExpression<ILamp>(context.Message.Filter, out var filter);

            await _serviceLamps.GetAll(filter)
                .ContinueWith(async t =>
                {
                    var lamps = t.IsCompletedSuccessfully ?
                        t.Result : Enumerable.Empty<IDevice>();

                    await context
                        .RespondAsync<IGetLampsResponse>(new
                        {
                            RequestId = context.Message.RequestId,
                            Lamps = lamps,
                            IsSuccess = t.IsCompletedSuccessfully,
                            ErrorMessage = t.Exception?.GetJoinedError()
                        });
                });


            //Expression<Func<IDevice, bool>>? filter = null;
            //Helper.TryConvertStringToExpression(context.Message.Filter, out filter);

            //await context.RespondAsync<IGetDevicesResponse>(new
            //{
            //    Devices = await _serviceDevices.GetAll(filter)
            //});
        }
    }


}
