using Common;
using Contracts;
using Contracts.V2;
using Grpc.Core;
using GRPCGateway.Helpers;
using Model;
using ModulePersons.API;

namespace GRPCGateway.Services
{
    public class PersonsServiceV2 : PersonIntegration.PersonIntegrationBase
    {
        private readonly ILogger<PersonsServiceV2> _logger;
        private readonly IServicePersonsMonolit _servicePersons;

        public PersonsServiceV2(ILogger<PersonsServiceV2> logger, 
            IServicePersonsMonolit servicePersons)
        {
            _logger = logger;
            _servicePersons = servicePersons;
        }

        public override async Task<Person> GetPersonById(LongMessage request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var person = _servicePersons
                    .GetById(request.Id).Result;

                return ConvertToMessage(person);
            });
        }

        public override async Task<Person> PersonAdd(Person request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var person = _servicePersons
                    .Add(ConvertFromMessage(request)).Result;

                return ConvertToMessage(person);
            });
        }

        public override async Task<PersonList> GetPersons(MessageString request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                if (!Helper.TryConvertStringToExpression<IPerson>(request.Filter, out var filterLamp))
                {
                    throw new Exception("Ошибка синтаксиса фильтра");
                }

                var persons = _servicePersons.GetAll(filterLamp).Result;
                var response = new PersonList();

                response.Persons
                    .AddRange(persons
                    .Select(x => ConvertToMessage(x)));

                return response;
            });
        }


        public override async Task<Person> PersonUpdate(Person request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var person = _servicePersons
                    .Update(ConvertFromMessage(request)).Result;

                return ConvertToMessage(person);
            });
        }

        public override async Task<EmptyMessage> PersonDelete(Person request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var person = _servicePersons
                    .Delete(ConvertFromMessage(request)).Result;

                return new EmptyMessage();
            });
        }

        internal static IPerson ConvertFromMessage(Person person)
        {
            var personConvert = IPerson.Create();
            personConvert.Id = person.Id;
            personConvert.FistName = person.FistName;
            personConvert.LastName = person.LastName;

            return personConvert;
        }

        internal static Person ConvertToMessage(IPerson person)
        {
            if (person == null)
                return new();

            return new Person
            {
                Id = person.Id,
                LastName = person.LastName,
                FistName = person.FistName,
            };
        }
    }

}
