using ModuleLampRoom.API;

namespace ModuleLampRoom.Interfaces
{
    /// <summary>
    /// Имитация БД
    /// </summary>
    internal interface ILampRoomStore 
    {
        Task<IEnumerable<IGivenPair>> GetAllGivenLamps();

        Task ChangeGivenLamp(IGivenPair givenPair, GivenLampStatus status);
    }
}
