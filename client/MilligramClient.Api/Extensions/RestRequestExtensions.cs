using MilligramClient.Common.Extensions;
using RestSharp;

namespace MilligramClient.Api.Extensions;

public static class RestRequestExtensions
{
    public static RestRequest AddParameterIfNotNull<TValue>(this RestRequest request, string name, TValue value)
    {
        return value is not null ? request.AddParameter(name, value.ToString()) : request;
    }

    public static RestRequest AddParametersIfNotNull<TValue>(this RestRequest request, string name, IEnumerable<TValue>? values)
    {
        return values.EmptyIfNull().Aggregate(request, (restRequest, value) => restRequest.AddParameterIfNotNull(name, value));
    }

    public static RestRequest AddQueryParameterIfNotNull<TValue>(this RestRequest request, string name, TValue value)
    {
        return value is not null ? request.AddQueryParameter(name, value.ToString()) : request;
    }

    public static RestRequest AddQueryParametersIfNotNull<TValue>(this RestRequest request, string name, IEnumerable<TValue>? values)
    {
        return values.EmptyIfNull().Aggregate(request, (restRequest, value) => restRequest.AddQueryParameterIfNotNull(name, value));
    }
}