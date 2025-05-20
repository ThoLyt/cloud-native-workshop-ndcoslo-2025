
using Dometrain.Monolith.Api.Sdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateApplicationBuilder();

host.Services.AddHttpClient("dometrain-api", client =>
{
    client.BaseAddress = new Uri("http://localhost:5148");
    client.DefaultRequestHeaders.Add("x-api-key", "ThisIsAlsoMeantToBeSecret");
});

host.Services.AddSingleton<ICoursesApiClient, CoursesApiClient>();
host.Services.AddSingleton<IStudentsApiClient, StudentsApiClient>();

var app = host.Build();

var coursesClient = app.Services.GetRequiredService<ICoursesApiClient>();
var studentsClient = app.Services.GetRequiredService<IStudentsApiClient>();

var course = await coursesClient.GetAsync("1b3c193d-b18c-44c8-a5b3-1727dc27d38c");
var student = await studentsClient.GetAsync("93cbd717-09bd-4c12-b2af-ee8d69a05637");


Console.WriteLine();
