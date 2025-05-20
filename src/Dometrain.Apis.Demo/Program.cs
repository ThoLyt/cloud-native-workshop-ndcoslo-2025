
using Dometrain.Monolith.Api.Sdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.CircuitBreaker;
//
// var host = Host.CreateApplicationBuilder();
//
// host.Services.AddDometrainApi("http://localhost:5148", "ThisIsAlsoMeantToBeSecret");
//
// var app = host.Build();
//
// var coursesClient = app.Services.GetRequiredService<ICoursesApiClient>();
// var studentsClient = app.Services.GetRequiredService<IStudentsApiClient>();
//
//
// var student = await studentsClient.GetAsync("93cbd717-09bd-4c12-b2af-ee8d69a05637");
// var course = await coursesClient.GetAsync("1b3c193d-b18c-44c8-a5b3-1727dc27d38c");
//
//
// Console.WriteLine();


var pipeline = new ResiliencePipelineBuilder()
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions
{
    BreakDuration = TimeSpan.FromSeconds(10),
    ShouldHandle = arguments => ValueTask.FromResult(
        arguments.Outcome.Exception is Exception),
    OnOpened = arguments =>
    {
        Console.WriteLine("Poof open");
        return ValueTask.CompletedTask;
    }
}).Build();


for (int i = 0; i < 100; i++)
{
    pipeline.Execute(PrintStuff);
}

Console.ReadKey();

void PrintStuff()
{
    var chance = Random.Shared.Next(1, 1000);

    if (chance > 800)
    {
        throw new Exception();
    }
    
    Console.WriteLine(chance);
}
