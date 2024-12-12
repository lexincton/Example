using Common;
using Model;

namespace ModuleLamps.API
{
    public interface IGetLampsRequest : IdRequest
    {
        string Filter { get; }
    }

    public interface IGetLampByIdRequest : IdRequest
    {
       long Id { get; set; }
    }

    public interface ILampActionRequest : IdRequest
    {
        CrudAction Action { get; }
        ILamp Lamp { get; }
    }

    public interface IGetLampByIdResponse : IResponse
    {
        ILamp Lamp { get; }
    }

    public interface IGetLampsResponse : IResponse
    {
        IEnumerable<ILamp> Lamps { get; }
    }

    public interface ILampActionResponse : IResponse
    {
        ILamp Lamp { get; }
    }
}
