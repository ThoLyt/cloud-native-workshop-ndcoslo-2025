using System.Net;
using System.Net.Http.Json;
using Dometrain.Monolith.Api.Contracts.Courses;

namespace Dometrain.Monolith.Api.Sdk;

public interface ICoursesApiClient
{
    Task<CourseResponse?> GetAsync(string idOrSlug);
}

public class CoursesApiClient : ICoursesApiClient
{
    private readonly IHttpClientFactory _clientFactory;

    public CoursesApiClient(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<CourseResponse?> GetAsync(string idOrSlug)
    {
        using var client = _clientFactory.CreateClient("dometrain-api");
        var response = await client.GetAsync($"courses/{idOrSlug}");
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<CourseResponse>();
    }
}
