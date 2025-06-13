using Zm.ServiceClient.Logging;

namespace Zm.ServiceClient.Clients;

public class JsonApiClientFactory(IHttpClientFactory httpClientFactory, IApiLogger logger)
{
    public JsonApiClient Create(string clientName)
    {
        var httpClient = httpClientFactory.CreateClient(clientName);
        if (httpClient.BaseAddress is null)
            throw new InvalidOperationException($"HttpClient '{clientName}' BaseAddress is not configured");

        return new JsonApiClient(httpClient, httpClient.BaseAddress.ToString(), logger);
    }
}