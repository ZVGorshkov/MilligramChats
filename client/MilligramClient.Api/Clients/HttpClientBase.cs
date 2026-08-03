using System.Text.Json;
using MilligramClient.Api.Exceptions;
using MilligramClient.Common.Extensions;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.Json;

namespace MilligramClient.Api.Clients;

public abstract class HttpClientBase : IDisposable
{
    private readonly RestClient _client;

    protected HttpClientBase(string address, TimeSpan timeout)
    {
        _client = new RestClient(address,
            options => options.Timeout = timeout,
            configureSerialization: config =>
                config.UseSystemTextJson(
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }));
    }

    protected Task<byte[]> SendRequestAsync(
        Method method,
        string path,
        CancellationToken cancellationToken)
    {
        return SendRequestAsync(method, path, null, null, cancellationToken);
    }

    protected Task<byte[]> SendRequestAsync(
        Method method,
        string path,
        string? token,
        CancellationToken cancellationToken)
    {
        return SendRequestAsync(method, path, token, null, cancellationToken);
    }

    protected Task<byte[]> SendRequestAsync(
        Method method,
        string path,
        Action<RestRequest>? setRequestParams,
        CancellationToken cancellationToken)
    {
        return SendRequestAsync(method, path, null, setRequestParams, cancellationToken);
    }

    protected async Task<byte[]> SendRequestAsync(
        Method method,
        string path,
        string? token,
        Action<RestRequest>? setRequestParams,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = CreateRequest(method, path, token, setRequestParams);
        var response = await _client.ExecuteAsync(request, cancellationToken);

        if (response.IsSuccessful && response.RawBytes?.Equals(null) == false)
            return response.RawBytes;

        var headers = response.Headers.EmptyIfNull().ToLookup(h => h.Name, h => h.Value);
        throw new SendRequestException(response.ResponseUri, response.Content, response.StatusCode, headers!, response.ErrorException);
    }

    protected Task<TResult> SendRequestAsync<TResult>(
        Method method,
        string path,
        CancellationToken cancellationToken)
    {
        return SendRequestAsync<TResult>(method, path, null, null, cancellationToken);
    }

    protected Task<TResult> SendRequestAsync<TResult>(
        Method method,
        string path,
        string? token,
        CancellationToken cancellationToken)
    {
        return SendRequestAsync<TResult>(method, path, token, null, cancellationToken);
    }

    protected Task<TResult> SendRequestAsync<TResult>(
        Method method,
        string path,
        Action<RestRequest>? setRequestParams,
        CancellationToken cancellationToken)
    {
        return SendRequestAsync<TResult>(method, path, null, setRequestParams, cancellationToken);
    }

    protected async Task<TResult> SendRequestAsync<TResult>(
        Method method,
        string path,
        string? token,
        Action<RestRequest>? setRequestParams,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = CreateRequest(method, path, token, setRequestParams);
        var response = await _client.ExecuteAsync<TResult>(request, cancellationToken);

        if (response.IsSuccessful && response.Data?.Equals(null) == false)
            return response.Data;

        var headers = response.Headers.EmptyIfNull().ToLookup(h => h.Name, h => h.Value);
        throw new SendRequestException<TResult>(response.ResponseUri, response.Content, response.StatusCode, headers!, response.Data, response.ErrorException);
    }

    private static RestRequest CreateRequest(Method method, string resource, string? token, Action<RestRequest>? setRequestParams)
    {
        var request = new RestRequest(resource, method);

        if (!token.IsNullOrWhiteSpace())
            request.Authenticator = new JwtAuthenticator(token);

        setRequestParams?.Invoke(request);
        return request;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}