using Dometrain.Cart.Api.ShoppingCarts;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace Dometrain.Cart.Processor;

public class ChangeFeedProcessorService : BackgroundService
{
    private const string DatabaseId = "cartdb";
    private const string SourceContainerId = "carts";
    private const string LeaseContainerId = "carts-leases";
    
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<ChangeFeedProcessorService> _logger;

    public ChangeFeedProcessorService(CosmosClient cosmosClient, ILogger<ChangeFeedProcessorService> logger)
    {
        _cosmosClient = cosmosClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var leaseContainer = _cosmosClient.GetContainer(DatabaseId, LeaseContainerId);

        var changeFeedProcessor = _cosmosClient.GetContainer(DatabaseId, SourceContainerId)
            .GetChangeFeedProcessorBuilder<ShoppingCart>("cache-processor", onChangesDelegate: HandleChangesAsync)
            .WithInstanceName($"cache-processor-{Guid.NewGuid().ToString()}")
            .WithLeaseContainer(leaseContainer)
            .Build();
        
        _logger.LogInformation("Starting Change Feed Processor");
        await changeFeedProcessor.StartAsync();
    }

    async Task HandleChangesAsync(
        ChangeFeedProcessorContext context,
        IReadOnlyCollection<ShoppingCart> changes,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Change Feed request consumed {RequestCharge} RU.", context.Headers.RequestCharge);

        
        foreach (ShoppingCart item in changes)
        {
            _logger.LogInformation(JsonConvert.SerializeObject(item));
            await Task.Delay(10);
        }
    }
}



