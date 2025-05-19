
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



builder.AddProject<Projects.Dometrain_Monolith_Api>("dometrain-api")
    .WithReference(mainDb).WaitFor(mainDb)
    .WithReference(cartDb).WaitFor(cartDb);

var app = builder.Build();

app.Run();
