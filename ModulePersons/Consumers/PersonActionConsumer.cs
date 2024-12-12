using Common;
using MassTransit;
using ModulePersons.API;
using ModulePersons.Interfaces;

namespace ModulePersons.Consumers
{
    internal class PersonActionConsumer : IConsumer<IPersonActionRequest>
    {
        private readonly IPersonsStore _servicePersons;
        public PersonActionConsumer(IPersonsStore servicePersons)
        {
            _servicePersons = servicePersons;
        }

        public async Task Consume(ConsumeContext<IPersonActionRequest> context)
        {
            var person = context.Message.Person;

            switch (context.Message.Action)
            {
                case CrudAction.Create:
                    person = await _servicePersons.Add(person);
                    break;
                case CrudAction.Update:
                    person = await _servicePersons.Update(person);
                    break;
                case CrudAction.Delete:
                    person = await _servicePersons.Delete(person);
                    break;
            }

            await context.RespondAsync<IPersonActionResponse>(new { Person = person });
        }
    }
}
