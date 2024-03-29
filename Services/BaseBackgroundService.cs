using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Coflnet.Sky.Bazaar.Flipper.Services;

public class BazaarFlipperBackgroundService : BackgroundService
{
    private IServiceScopeFactory scopeFactory;
    private IConfiguration config;
    private ILogger<BazaarFlipperBackgroundService> logger;
    private Prometheus.Counter consumeCount = Prometheus.Metrics.CreateCounter("sky_bazaar_flipper_conume", "How many messages were consumed");

    public BazaarFlipperBackgroundService(
        IServiceScopeFactory scopeFactory, IConfiguration config, ILogger<BazaarFlipperBackgroundService> logger)
    {
        this.scopeFactory = scopeFactory;
        this.config = config;
        this.logger = logger;
    }
    /// <summary>
    /// Called by asp.net on startup
    /// </summary>
    /// <param name="stoppingToken">is canceled when the applications stops</param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var flipCons = Kafka.KafkaConsumer.ConsumeBatch<dev.BazaarPull>(config, config["TOPICS:BAZAAR"], async batch =>
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<BazaarFlipperService>();
            var bookService = scope.ServiceProvider.GetRequiredService<BookFlipService>();
            foreach (var lp in batch)
            {
                await service.BazaarUpdate(lp);
                await bookService.BookUpdate(lp);
            }
            consumeCount.Inc(batch.Count());
        }, stoppingToken, "sky-bazaar-flipper+" + GetHostName(), 5, AutoOffsetReset.Latest);
        logger.LogInformation("Started Bazaar Flipper Background Service");
        await Task.WhenAll(flipCons);
    }

    private string GetHostName()
    {
        return System.Net.Dns.GetHostName();
    }
}