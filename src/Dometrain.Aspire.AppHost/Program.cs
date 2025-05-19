
var builder = DistributedApplication.CreateBuilder(args);


var mainDbUsername = builder.AddParameter("postgres-username");
var mainDbPassword = builder.AddParameter("postgres-password");

var mainDb = builder.AddPostgres("main-db", mainDbUsername, mainDbPassword, 5432)
    .WithImage("postgres:latest")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("dometrain");


var cartDb = builder.AddAzureCosmosDB("cosmosdb")
    //.RunAsPreviewEmulator()
    .AddCosmosDatabase("cartdb");

var redis = builder.AddRedis("redis")
    .WithImage("redis:latest")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight();

builder.AddProject<Projects.Dometrain_Monolith_Api>("dometrain-api")
    .WithReference(mainDb).WaitFor(mainDb)
    .WithReference(redis).WaitFor(redis)
    .WithReference(cartDb).WaitFor(cartDb);

var app = builder.Build();

app.Run();
