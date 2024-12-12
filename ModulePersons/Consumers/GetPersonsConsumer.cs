using System.Linq.Expressions;
using Common;
using MassTransit;
using Model;
using ModulePersons.API;
using ModulePersons.Interfaces;

namespace ModulePersons.Consumers
{
    internal class GetPersonsConsumer : IConsumer<IGetPersonsRequest>
    {
        private readonly IPersonsStore _servicePersons;
        public GetPersonsConsumer(IPersonsStore servicePersons)
        {
            _servicePersons = servicePersons;
        }

        public async Task Consume(ConsumeContext<IGetPersonsRequest> context)
        {
            Helper.TryConvertStringToExpression<IPerson>(context.Message.Filter, out var filter);

            await _servicePersons.GetAll(filter)
                .ContinueWith(async t =>
                {
                    var persons = t.IsCompletedSuccessfully ?
                        t.Result : Enumerable.Empty<IPerson>();

                    await context
                        .RespondAsync<IGetPersonsResponse>(new
                        {
                            RequestId = context.Message.RequestId,
                            Persons = persons,
                            IsSuccess = t.IsCompletedSuccessfully,
                            ErrorMessage = t.Exception?.GetJoinedError()
                        });
                });
        }
    }

}
