using System.Net;

namespace MilligramClient.Api.Exceptions;

public class SendRequestException<TResponseData> : SendRequestException
{
    public TResponseData? ResponseData { get; }

    public SendRequestException(
        Uri? uri,
        string? content,
        HttpStatusCode statusCode,
        ILookup<string?, string?> headers,
        TResponseData? responseData,
        Exception? innerException)
        : base(uri, content, statusCode, headers, innerException)
    {
        ResponseData = responseData;
    }
}