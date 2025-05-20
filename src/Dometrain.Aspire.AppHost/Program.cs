
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

    var rabbitMq = builder.AddRabbitMQ("rabbitmq")
        .WithLifetime(ContainerLifetime.Persistent)
        .WithManagementPlugin();

    builder.AddContainer("prometheus", "prom/prometheus")
        .WithBindMount("../../prometheus", "/etc/prometheus", true)
        .WithLifetime(ContainerLifetime.Persistent)
        .WithHttpEndpoint(port: 9090, targetPort: 9090);

    var grafana = builder.AddContainer("grafana", "grafana/grafana")
        .WithLifetime(ContainerLifetime.Persistent)
        .WithBindMount("../../grafana/config", "/etc/grafana", isReadOnly: true)
        .WithBindMount("../../grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
        .WithHttpEndpoint(targetPort: 3000, name: "http");

var mainApi = builder.AddProject<Projects.Dometrain_Monolith_Api>("dometrain-api")
    .WithReplicas(1)
    .WithReference(mainDb).WaitFor(mainDb)
    .WithReference(redis).WaitFor(redis)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("http"));

builder.AddProject<Projects.Dometrain_Cart_Processor>("dometrain-cart-processor")
    .WithReference(redis).WaitFor(redis)
    .WithReference(cartDb).WaitFor(cartDb);

builder.AddProject<Projects.Dometrain_Cart_Api>("cart-api")
    .WithReference(redis).WaitFor(redis)
    .WithReference(cartDb).WaitFor(cartDb)
    .WithReference(mainApi).WaitFor(mainApi)
    .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("http"));
    //.WithEnvironment("MainApi__BaseUrl", mainApi.GetEndpoint("http"));


var app = builder.Build();

app.Run();
