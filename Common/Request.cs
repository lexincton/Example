namespace Common
{
    public interface IdRequest
    {
        Guid RequestId { get; }
    }

    public interface IResponse : IdRequest
    {
        bool IsSuccess { get; }
        string ErrorMessage { get; }
    }
}
