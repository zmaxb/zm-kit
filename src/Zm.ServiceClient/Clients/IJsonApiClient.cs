using Zm.Common.Models;

namespace Zm.ServiceClient.Clients;

public interface IJsonApiClient
{
    Task<ApiResponse<T>> GetJsonAsync<T>(string url, CancellationToken cancellationToken = default);

    Task<ApiResponse<TResponse>> PostJsonAsync<TRequest, TResponse>(
        string url,
        TRequest data,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<TResponse>> PutJsonAsync<TRequest, TResponse>(
        string url,
        TRequest data,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<T>> DeleteAsync<T>(string url, CancellationToken cancellationToken = default);
}