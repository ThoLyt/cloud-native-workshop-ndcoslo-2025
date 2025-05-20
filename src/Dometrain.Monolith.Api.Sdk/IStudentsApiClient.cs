using System.Net;
using System.Net.Http.Json;
using Dometrain.Monolith.Api.Contracts.Students;

namespace Dometrain.Monolith.Api.Sdk;

public interface IStudentsApiClient
{
    Task<StudentResponse?> GetAsync(string idOrEmail);
}

public class StudentsApiClient : IStudentsApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public StudentsApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<StudentResponse?> GetAsync(string idOrEmail)
    {
        var client = _httpClientFactory.CreateClient("dometrain-api");
        var response = await client.GetAsync($"students/{idOrEmail}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<StudentResponse>();
    }
}
