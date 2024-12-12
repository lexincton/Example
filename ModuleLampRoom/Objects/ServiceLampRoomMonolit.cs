using ModuleLampRoom.API;
using ModuleLampRoom.Interfaces;
using ModuleLamps.Objects;
using ModulePersons.API;

namespace ModuleLampRoom.Objects
{
    internal class ServiceLampRoomMonolit : IServiceLampRoomMonolit
    {
        private readonly ILampRoomStore _lampRoomStore;
        private readonly IServicePersonsMonolit _servicePersons;
        private readonly IServiceLampsMonolit _serviceLamps;


        public ServiceLampRoomMonolit(
            ILampRoomStore lampRoomStore,
            IServicePersonsMonolit servicePersons,
            IServiceLampsMonolit serviceLamps
            )
        {
            _lampRoomStore = lampRoomStore;
            _servicePersons = servicePersons;
            _serviceLamps = serviceLamps;
        }

        public async Task ChangeGivenLamp(IGivenPair givenPair, GivenLampStatus status)
        {
            await _serviceLamps.GetById(givenPair.LampId);

            await _servicePersons.GetById(givenPair.PersonId);

            await _lampRoomStore.ChangeGivenLamp(givenPair, status); ;
        }

        public async Task<IEnumerable<IGivenPairFilled>> GetAllGivenLamps()
        {
            return await _lampRoomStore.GetAllGivenLamps()
                .ContinueWith(t =>
                {
                    if (!t.IsCompletedSuccessfully)
                        return Enumerable.Empty<IGivenPairFilled>();

                    var lampIds = t.Result
                        .Select(x => x.LampId)
                        .ToArray();

                    var lamps = _serviceLamps
                        .GetAll(x => lampIds.Contains(x.Id))
                        .Result;

                    var personIds = t.Result
                        .Select(x => x.PersonId)
                        .ToArray();

                    var persons = _servicePersons
                        .GetAll(x => lampIds.Contains(x.Id))
                        .Result;

                    return t.Result
                        .Select(x => IGivenPairFilled.Create
                        (
                            persons.FirstOrDefault(p=> p.Id == x.PersonId),
                            lamps.FirstOrDefault(p => p.Id == x.LampId)
                        ));
                });
        }
    }
}
