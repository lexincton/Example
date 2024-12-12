using ModuleLampRoom.API;

namespace ModuleLampRoom
{
    public interface IServiceLampRoomMonolit
    {
        Task<IEnumerable<IGivenPairFilled>> GetAllGivenLamps();

        Task ChangeGivenLamp(IGivenPair givenPair, GivenLampStatus status);
    }
}
