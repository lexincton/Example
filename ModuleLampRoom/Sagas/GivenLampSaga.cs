using Common;
using MassTransit;
using Microsoft.Extensions.Logging;
using ModuleLampRoom.API;
using ModuleLampRoom.Interfaces;
using ModuleLamps.API;
using ModulePersons.API;

namespace ModuleLampRoom.Sagas
{
    internal sealed class GivenLampSaga : MassTransitStateMachine<GivenLampSagaState>
    {
        private readonly ILogger<GivenLampSaga> _logger;
        private readonly ILampRoomStore _serviceLampRoom;

        public State WaitingDevice { get; private set; }
        public State WaitingPerson { get; private set; }
        public State Faulted { get; private set; }
        public State Success { get; private set; }

        public Event<IGivenLampRequest> GivenLampRequest { get; set; }
        public Event<IGetLampByIdResponse> GetLamp { get; set; }
        public Event<IGetPersonByIdResponse> GetPerson { get; set; }

        public GivenLampSaga(
            ILogger<GivenLampSaga> logger,
            ILampRoomStore serviceLampRoom
            )
        {
            _logger = logger;
            _serviceLampRoom = serviceLampRoom;

            InstanceState(x => x.CurrentState);
            Event(() => GivenLampRequest, x => x.CorrelateById(y => y.Message.RequestId)
                .SelectId(x => Guid.NewGuid()));
            Event(() => GetLamp, x => x.CorrelateById(y => y.Message.RequestId));
            Event(() => GetPerson, x => x.CorrelateById(y => y.Message.RequestId));

            Initially(
                When(GivenLampRequest)
                .Then(x =>
                {
                    if (!x.TryGetPayload(out SagaConsumeContext<GivenLampSagaState, IGivenLampRequest> payload))
                        throw new Exception("Error TryGetPayload");

                    x.Saga.RequestId = payload.RequestId;
                    x.Saga.ResponseAddress = payload.ResponseAddress;
                    x.Saga.GivenPair = x.Message.GivenPair;
                    x.Saga.Status = x.Message.Status;
                })
                .Publish(x => x.Init<IGetLampByIdRequest>(new
                {
                    RequestId = x.Saga.CorrelationId,
                    Id = x.Saga.GivenPair.LampId,
                }).Result.Message)
                .TransitionTo(WaitingDevice));

            During(WaitingDevice,
                When(GetLamp)
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
                                await cnt.TransitionToState(WaitingPerson);
                                await cnt.Publish(cnt.Init<IGetPersonByIdRequest>(new
                                {
                                    RequestId = cnt.Saga.CorrelationId,
                                    Id = cnt.Saga.GivenPair.PersonId,
                                }).Result.Message);
                            }
                        }));

            During(WaitingPerson,
                When(GetPerson)
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
                           await _serviceLampRoom
                               .ChangeGivenLamp(cnt.Saga.GivenPair, cnt.Saga.Status)
                               .ContinueWith(async t=>
                               {
                                   await cnt.TransitionToState(
                                       t.IsCompletedSuccessfully? Success : Faulted);

                                   await RespondFromSaga(cnt, t.Exception?.GetJoinedError());
                               });
                       }
                   }));
        }


        private async Task RespondFromSaga<T>(BehaviorContext<GivenLampSagaState, T> context,
            string error = "") where T : class, IResponse
        {
            var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);

            var isSuccess = string.IsNullOrEmpty(error);
            var errorMessage = error;

            if (context.Message is IResponse responseMsg)
            {
                isSuccess = string.IsNullOrEmpty(error) ?
                    responseMsg.IsSuccess : false;

                errorMessage = string.IsNullOrEmpty(error) ?
                    responseMsg.ErrorMessage : error;
            }

            await endpoint.Send<IGivenLampResponse>(new
            {
                RequestId = context.Saga.RequestId,
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage,
            }, r => r.RequestId = context.Saga.RequestId);
        }
    }
}
