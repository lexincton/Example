using ModuleLampRoom.API;
using ModuleLampRoom.Interfaces;

namespace ModuleLampRoom.Objects
{
    internal class LampRoomStore : ILampRoomStore
    {
        private readonly IList<IGivenPair> _givenList = new List<IGivenPair>();

        public Task ChangeGivenLamp(IGivenPair givenPair, GivenLampStatus status)
        {
            return Task.Run(() =>
            {
                var givenLoc = _givenList
                    .FirstOrDefault(x => x.LampId == givenPair.LampId &&
                        x.PersonId == givenPair.PersonId);

                switch (status)
                {
                    case GivenLampStatus.Given:
                        if (givenLoc != null)
                            throw new ArgumentException($"Светильник уже выдан");

                        _givenList.Add(givenPair);
                        break;
                    case GivenLampStatus.Return:
                        if (givenLoc == null)
                            throw new ArgumentException($"Указанная пара отсутствует");

                        _givenList.Remove(givenLoc);
                        break;
                }
            });
        }

        public Task<IEnumerable<IGivenPair>> GetAllGivenLamps()
        {
            return Task.FromResult(_givenList.AsEnumerable());
        }
    }
}
