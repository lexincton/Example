using Model;
using ModuleLampRoom.API;

namespace ModuleLampRoom.Sagas
{
    public sealed class GetGivenLampsState : MachineStateBaseState
    {
        public IEnumerable<IGivenPair> GivenPairs { get; set; } =
            Enumerable.Empty<IGivenPair>();

        public string FilterPersons { get; set; } = string.Empty;
        public string FilterLamps { get; set; } = string.Empty;

        public IEnumerable<ILamp> Lamps { get; set; } = Enumerable.Empty<ILamp>();
        public IEnumerable<IPerson> Persons { get; set; } = Enumerable.Empty<IPerson>();

        public IEnumerable<IGivenPairFilled> GivensFilled
        {
            get
            {
                var givens = new List<IGivenPairFilled>();

                foreach (var givenPair in GivenPairs)
                {
                    var given = IGivenPairFilled.Create(
                        Persons.FirstOrDefault(x => x.Id == givenPair.PersonId),
                        Lamps.FirstOrDefault(x => x.Id == givenPair.LampId)
                        );
                    givens.Add(given);
                }

                return givens;
            }
        }
    }
}
