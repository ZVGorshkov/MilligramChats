using System.Net;

namespace MilligramServer.Exceptions;

public abstract class ApiExceptionBase : Exception
{
    public abstract HttpStatusCode Code { get; }

    protected ApiExceptionBase(string? message, Exception? exception)
        : base(message ?? string.Empty, exception)
    {
    }
}