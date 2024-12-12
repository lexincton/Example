using Common;
using Model;

namespace ModulePersons.API
{
    public interface IGetPersonsRequest : IdRequest
    {
        string Filter { get; }
    }

    public interface IGetPersonByIdRequest : IdRequest
    {
        long Id { get; }
    }

    public interface IGetPersonByIdResponse : IResponse
    {
        long Id { get; }
    }


    public interface IGetPersonsResponse : IResponse
    {
        IEnumerable<IPerson> Persons { get; }
    }

    public interface IPersonActionRequest : IdRequest
    {
        CrudAction Action { get; }
        IPerson Person { get; }
    }

    public interface IPersonActionResponse : IResponse
    {
        IPerson Person { get; }
    }
}
