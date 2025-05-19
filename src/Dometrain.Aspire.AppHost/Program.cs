
using Aspire.Hosting.Azure;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);


var mainDbUsername = builder.AddParameter("postgres-username");
var mainDbPassword = builder.AddParameter("postgres-password");

var mainDb = builder.AddPostgres("main-db", mainDbUsername, mainDbPassword, 5432)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("dometrain");


IResourceBuilder<AzureCosmosDBDatabaseResource> cartDb;
// if (builder.Environment.IsDevelopment())
// {
// #pragma warning disable ASPIRECOSMOSDB001
//     cartDb = builder.AddAzureCosmosDB("cosmosdb")
//         .RunAsPreviewEmulator(resourceBuilder =>
//         {
//             resourceBuilder.WithDataExplorer().WithLifetime(ContainerLifetime.Persistent);
//         })
// #pragma warning restore ASPIRECOSMOSDB001
//         .AddCosmosDatabase("cartdb");
// }
// else
// {
    cartDb = builder.AddAzureCosmosDB("cosmosdb")
        .AddCosmosDatabase("cartdb");
//}

    var redis = builder.AddRedis("redis")
        .WithLifetime(ContainerLifetime.Persistent)
        .WithRedisInsight();


builder.AddProject<Projects.Dometrain_Monolith_Api>("dometrain-api")
    .WithReplicas(5)
    .WithReference(mainDb).WaitFor(mainDb)
    .WithReference(redis).WaitFor(redis)
    .WithReference(cartDb).WaitFor(cartDb);

var app = builder.Build();

app.Run();
