﻿using MassTransit;

namespace ModuleLampRoom.Sagas
{
    public abstract class MachineStateBaseState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string? CurrentState { get; set; }
        public Guid? RequestId { get; set; }
        public Uri? ResponseAddress { get; set; }
    }
}
