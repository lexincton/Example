using Contracts;
using Contracts.V1;
using Grpc.Core;
using GRPCGateway.Helpers;
using MassTransit;
using Model;
using ModulePersons.API;

namespace GRPCGateway.Services
{
    public class PersonsServiceV1 : PersonIntegration.PersonIntegrationBase
    {
        private readonly ILogger<PersonsServiceV1> _logger;
        private readonly IBus _bus;

        public PersonsServiceV1(ILogger<PersonsServiceV1> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public override async Task<Person> GetPersonById(LongMessage request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var resultRequest = _bus.Request<IGetPersonByIdRequest, IPersonActionResponse>(new { request.Id })
                    .Result.Message;

                resultRequest.ThrowIfNotSuccess();

                return ConvertToMessage(resultRequest.Person);
            });
        }

        public override async Task<PersonList> GetPersons(MessageString request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var resultRequest = _bus
                    .Request<IGetPersonsRequest, IGetPersonsResponse>(new { request.Filter })
                    .Result.Message;

                resultRequest.ThrowIfNotSuccess();

                var response = new PersonList();

                response.Persons
                    .AddRange(resultRequest.Persons
                    .Select(x => ConvertToMessage(x)));

                return response;
            });
        }

        public override async Task<Person> PersonAdd(Person request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var response = _bus.Request<IPersonActionRequest, IPersonActionResponse>(new
                {
                    Action = Common.CrudAction.Create,
                    Person = ConvertFromMessage(request)
                }).Result.Message;

                response.ThrowIfNotSuccess();

                return ConvertToMessage(response.Person);
            });
        }

        public override async Task<Person> PersonUpdate(Person request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context,
                () =>
            {
                var response = _bus.Request<IPersonActionRequest, IPersonActionResponse>(new
                {
                    Action = Common.CrudAction.Update,
                    Person = ConvertFromMessage(request)
                }).Result.Message;

                response.ThrowIfNotSuccess();

                return ConvertToMessage(response.Person);
            });
        }

        public override async Task<EmptyMessage> PersonDelete(Person request, ServerCallContext context)
        {
            return await IntegrationHelper.ActionExecute(_logger, context, () =>
            {
                var response = _bus.Request<IPersonActionRequest, IPersonActionResponse>(new
                {
                    Action = Common.CrudAction.Delete,
                    Person = ConvertFromMessage(request)
                }).Result.Message;

                response.ThrowIfNotSuccess();

                return new EmptyMessage();
            });
        }

        internal static object ConvertFromMessage(Person person)
        {
            return new
            {
                person.Id,
                person.LastName,
                person.FistName,
            };
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
