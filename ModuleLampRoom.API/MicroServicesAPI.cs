
using Common;
using Model;

namespace ModuleLampRoom.API
{
    public enum GivenLampStatus
    {
        Given,
        Return
    }

    public interface IGetGivenLampsRequest : IdRequest
    {
    }


    public interface IGivenLampRequest : IdRequest
    {
        IGivenPair GivenPair { get; }
        GivenLampStatus Status { get; }
    }

    public interface IGivenLampResponse : IResponse
    {
    }

    public interface IGetGivenLampsResponse : IResponse
    {
        IEnumerable<IGivenPairFilled> Givens { get; }
    }

    public interface IGivenPairFilled
    {
        IPerson Person { get; }
        ILamp Lamp { get; }

        public static IGivenPairFilled Create(IPerson person, ILamp lamp)
        {
            return new GivenPairLoaded(person, lamp);
        }


        class GivenPairLoaded : IGivenPairFilled
        {
            public IPerson Person { get; }

            public ILamp Lamp { get; }

            public GivenPairLoaded(IPerson person, ILamp lamp)
            {
                Person = person;
                Lamp = lamp;
            }
        }
    }



    public interface IGivenPair
    {
        long PersonId { get; }
        long LampId { get; }

        public static IGivenPair Create(long personId, long lampId)
        {
            return new GivenPair(personId, lampId);
        }

        string ToString();


        class GivenPair : IGivenPair
        {
            public long PersonId { get; }
            public long LampId { get; }

            public GivenPair(long personId, long lampId)
            {
                PersonId = personId;
                LampId = lampId;
            }
        }
    }
}
