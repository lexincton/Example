using Common;
using MassTransit;
using ModulePersons.API;
using ModulePersons.Interfaces;

namespace ModulePersons.Consumers
{
    internal class GetPersonByIdConsumer : IConsumer<IGetPersonByIdRequest>
    {
        private readonly IPersonsStore _servicePersons;
        public GetPersonByIdConsumer(IPersonsStore servicePersons)
        {
            _servicePersons = servicePersons;
        }

        public async Task Consume(ConsumeContext<IGetPersonByIdRequest> context)
        {
            await _servicePersons.GetById(context.Message.Id)
                .ContinueWith(async t =>
                {
                    var person = t.IsCompletedSuccessfully ? t.Result : null;

                    await context
                        .RespondAsync<IGetPersonByIdResponse>(new
                        {
                            RequestId = context.Message.RequestId,
                            Person = person,
                            IsSuccess = t.IsCompletedSuccessfully,
                            ErrorMessage = t.Exception?.GetJoinedError()
                        });
                });
        }
    }
}
