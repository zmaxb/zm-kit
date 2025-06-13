using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Zm.Common.Models;
using Zm.ServiceClient.Logging;

namespace Zm.ServiceClient.Clients;

public class JsonApiClient(HttpClient httpClient, string baseUrl, IApiLogger? logger = null) : IJsonApiClient
{
    private readonly string _baseUrl = baseUrl.TrimEnd('/');

    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<ApiResponse<T>> GetJsonAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        var fullUrl = CombineUrl(url);
        return await ExecuteAsync<T>(() => httpClient.GetAsync(fullUrl, cancellationToken), "GET", fullUrl, null,
            cancellationToken);
    }

    public async Task<ApiResponse<TResponse>> PostJsonAsync<TRequest, TResponse>(
        string url,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        var fullUrl = CombineUrl(url);
        var jsonBody = SafeSerialize(data, cancellationToken);

        var request = new HttpRequestMessage(HttpMethod.Post, fullUrl)
        {
            Content = new StringContent(jsonBody ?? "", Encoding.UTF8, "application/json")
        };

        return await ExecuteAsync<TResponse>(() =>
            httpClient.SendAsync(request, cancellationToken), "POST", fullUrl, jsonBody, cancellationToken);
    }

    public async Task<ApiResponse<TResponse>> PutJsonAsync<TRequest, TResponse>(string url, TRequest data,
        CancellationToken cancellationToken = default)
    {
        var fullUrl = CombineUrl(url);
        var jsonBody = SafeSerialize(data, cancellationToken);

        return await ExecuteAsync<TResponse>(async () =>
        {
            var response = await httpClient.PutAsJsonAsync(fullUrl, data, cancellationToken);
            return response;
        }, "PUT", fullUrl, jsonBody, cancellationToken);
    }

    public async Task<ApiResponse<T>> DeleteAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        var fullUrl = CombineUrl(url);
        return await ExecuteAsync<T>(()
                => httpClient.DeleteAsync(fullUrl, cancellationToken), "DELETE", fullUrl, null,
            cancellationToken);
    }

    private async Task<ApiResponse<T>> ExecuteAsync<T>(
        Func<Task<HttpResponseMessage>> httpAction,
        string method,
        string url,
        string? requestBody = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger?.LogDebug($"Sending {method} request to {url}", requestBody);

            var response = await httpAction();
            response.EnsureSuccessStatusCode();

            var result =
                await response.Content.ReadFromJsonAsync<ApiResponse<T>>(JsonSerializerOptions, cancellationToken);

            if (result is null)
            {
                logger?.LogWarning($"Empty response received from {url}", requestBody);
                return ApiResponse<T>.Fail("Server returned empty response.");
            }

            stopwatch.Stop();
            logger?.LogInformation($"Request {method} {url} completed in {stopwatch.ElapsedMilliseconds} ms");

            return result;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            logger?.LogWarning($"Request {method} {url} was cancelled after {stopwatch.ElapsedMilliseconds} ms");
            return ApiResponse<T>.Fail("Request was cancelled.");
        }
        catch (HttpRequestException httpEx)
        {
            stopwatch.Stop();
            logger?.LogError($"HTTP error during {method} to {url} after {stopwatch.ElapsedMilliseconds} ms", httpEx,
                requestBody);
            return ApiResponse<T>.Fail($"HTTP error: {httpEx.Message}");
        }
        catch (JsonException jsonEx)
        {
            stopwatch.Stop();
            logger?.LogError($"JSON parsing error from {url} after {stopwatch.ElapsedMilliseconds} ms", jsonEx,
                requestBody);
            return ApiResponse<T>.Fail($"JSON parse error: {jsonEx.Message}");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger?.LogError($"Unexpected error during {method} to {url} after {stopwatch.ElapsedMilliseconds} ms", ex,
                requestBody);
            return ApiResponse<T>.Fail($"Unexpected error: {ex.Message}");
        }
    }

    private string CombineUrl(string url)
        => $"{_baseUrl.TrimEnd('/')}/{url.TrimStart('/')}";

    private string? SafeSerialize<T>(T? data, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return data is null ? null : JsonSerializer.Serialize(data);
        }
        catch (OperationCanceledException)
        {
            logger?.LogWarning("Serialization was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogWarning("Failed to serialize request body", ex);
            return "<<< Unable to serialize request body >>>";
        }
    }
}