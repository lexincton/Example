using ModuleLampRoom.API;
using ModuleLamps.API;

namespace ModuleLampRoom.Sagas
{
    public sealed class GivenLampSagaState : MachineStateBaseState
    {
        public IGivenPair GivenPair { get; set; }
        public GivenLampStatus Status { get; set; }
    }
}
