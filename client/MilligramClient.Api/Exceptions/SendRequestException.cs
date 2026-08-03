using System.Net;

namespace MilligramClient.Api.Exceptions;

public class SendRequestException : Exception
{
    public Uri? Uri { get; }
    public string? Content { get; }
    public HttpStatusCode StatusCode { get; }
    public ILookup<string?, string?> Headers { get; }

    public SendRequestException(
        Uri? uri,
        string? content,
        HttpStatusCode statusCode,
        ILookup<string?, string?> headers,
        Exception? innerException)
        : base($"Send request failed. Url: {uri}, StatusCode: {statusCode}", innerException)
    {
        Uri = uri;
        Content = content;
        StatusCode = statusCode;
        Headers = headers;
    }
}