using Common;
using MassTransit;
using Microsoft.Extensions.Logging;
using Model;
using ModuleLampRoom.API;
using ModuleLampRoom.Interfaces;
using ModuleLamps.API;
using ModulePersons.API;

namespace ModuleLampRoom.Sagas
{
    internal sealed class GetGivenLampsSaga : MassTransitStateMachine<GetGivenLampsState>
    {
        private readonly ILogger<GivenLampSaga> _logger;
        private readonly ILampRoomStore _lampRoomStore;

        public State WaitingLamps { get; private set; }
        public State WaitingPersons { get; private set; }
        public State Faulted { get; private set; }
        public State Success { get; private set; }

        public Event<IGetGivenLampsRequest> GivenLampsRequest { get; set; }
        public Event<IGetLampsResponse> GetLamps { get; set; }
        public Event<IGetPersonsResponse> GetPersons { get; set; }

        public GetGivenLampsSaga(
            ILogger<GivenLampSaga> logger,
            ILampRoomStore lampRoomStore
            )
        {
            _logger = logger;
            _lampRoomStore = lampRoomStore;

            InstanceState(x => x.CurrentState);
            Event(() => GivenLampsRequest, x => x.CorrelateById(y => y.Message.RequestId)
                .SelectId(x => Guid.NewGuid()));
            Event(() => GetLamps, x => x.CorrelateById(y => y.Message.RequestId));
            Event(() => GetPersons, x => x.CorrelateById(y => y.Message.RequestId));

            Initially(                
                When(GivenLampsRequest)
                .ThenAsync(async cnt =>
                {
                    if (!cnt.TryGetPayload(out SagaConsumeContext<GetGivenLampsState, IGetGivenLampsRequest> payload))
                        throw new Exception("Error TryGetPayload");

                    cnt.Saga.RequestId = payload.RequestId;
                    cnt.Saga.ResponseAddress = payload.ResponseAddress;
                    cnt.Saga.GivenPairs = await _lampRoomStore.GetAllGivenLamps();

                    var lampIds = cnt.Saga.GivenPairs
                        .Select(x => x.LampId)
                        .Distinct()
                        .ToArray();

                    if (!Helper.TryConvertExpressionToString<IDevice>(p => lampIds.Contains(p.Id), out var filterLamp))
                    {
                        await RespondFromSaga(cnt, "Ошибка синтаксиса фильтра");
                    }

                    cnt.Saga.FilterLamps = filterLamp;
                })
                .Publish(x => x.Init<IGetLampsRequest>(new
                {
                    RequestId = x.Saga.CorrelationId,
                    Filter = x.Saga.FilterLamps
                }).Result.Message)
                .TransitionTo(WaitingLamps));

            During(WaitingLamps,
                When(GetLamps)
                   .ThenAsync(async cnt =>
                        {
                            _logger.LogInformation(cnt.Message.IsSuccess.ToString());
                            if (!cnt.Message.IsSuccess)
                            {
                                await cnt.TransitionToState(Faulted);
                                await RespondFromSaga(cnt);
                            }
                            else
                            {
                                cnt.Saga.Lamps = cnt.Message.Lamps;

                                var personIds = cnt.Saga.GivenPairs
                                    .Select(x => x.PersonId)
                                    .Distinct()
                                    .ToArray();

                                if (!Helper.TryConvertExpressionToString<IPerson>(p => 
                                    personIds.Contains(p.Id), out var filterPerson))
                                {
                                    await RespondFromSaga(cnt, "Ошибка синтаксиса фильтра");
                                }
                                cnt.Saga.FilterPersons = filterPerson;
                            }
                        })
                   .Publish(x => x.Init<IGetPersonsRequest>(new
                   {
                        RequestId = x.Saga.CorrelationId,
                        Filter = x.Saga.FilterPersons
                   }).Result.Message)
                .TransitionTo(WaitingPersons)); ;

            During(WaitingPersons,
                When(GetPersons)
                   .ThenAsync(async cnt =>
                   {
                       _logger.LogInformation(cnt.Message.IsSuccess.ToString());

                       await cnt.TransitionToState(cnt.Message.IsSuccess ? Success : Faulted);

                       if (!cnt.Message.IsSuccess)
                       {
                           await cnt.TransitionToState(Faulted);
                           await RespondFromSaga(cnt);
                       }
                       else
                       {
                           cnt.Saga.Persons = cnt.Message.Persons;
                           await RespondFromSaga(cnt);
                       }
                   }));
        }

        private async Task RespondFromSaga<T>(BehaviorContext<GetGivenLampsState, T> context, 
            string error = "") where T : class
        {
            var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);

            var isSuccess = string.IsNullOrEmpty(error);
            var errorMessage = error;

            if (context.Message is IResponse responseMsg)
            {
                isSuccess = string.IsNullOrEmpty(error)?
                    responseMsg.IsSuccess : false;

                errorMessage = string.IsNullOrEmpty(error)?
                    responseMsg.ErrorMessage : error;
            }

            await endpoint.Send<IGetGivenLampsResponse>(new
            {
                RequestId = context.Saga.RequestId,
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage,
                Givens = context.Saga.GivensFilled,
            }, r => r.RequestId = context.Saga.RequestId);
        }
    }
}
