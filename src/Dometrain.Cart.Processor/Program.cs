

using Dometrain.Cart.Processor;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureCosmosClient("cartdb");
builder.AddRedisClient("redis");

builder.Services.AddHostedService<ChangeFeedProcessorService>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();
