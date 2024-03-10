using System.Threading.Tasks;
using Coflnet.Sky.Bazaar.Flipper.Models;
using System;
using System.Linq;
using dev;
using Coflnet.Sky.Bazaar.Client.Api;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Coflnet.Sky.Bazaar.Flipper.Services;
public class BazaarFlipperService
{
    private const double AverageuserFees = 0.0125;
    private IBazaarApi client;
    private Dictionary<string, BazaarFlip> flips = new Dictionary<string, BazaarFlip>();
    private ILogger<BazaarFlipperService> logger;
    private DateTime lastUpdate = DateTime.MinValue;

    public BazaarFlipperService(IBazaarApi client, ILogger<BazaarFlipperService> logger)
    {
        this.client = client;
        this.logger = logger;
    }

    /// <summary>
    /// Gets called whenever new bazaar data is available
    /// Should finish within 10 seconds to be called with the next bazaar update
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    internal async Task BazaarUpdate(BazaarPull update)
    {
        foreach (var item in update.Products)
        {
            var status = item.QuickStatus;
            var volume = Math.Min(status.BuyMovingWeek, status.SellMovingWeek);
            if (volume == 0)
            {
                continue;
            }
            // fees are deducted when displaying since they can differ between players
            var spread = status.BuyPrice - status.SellPrice;
            var coinsPerWeek = volume * spread;
            var coinsPerHour = coinsPerWeek / 168;
            var hourlyVolume = (double)volume / 168;
            var fees = status.BuyPrice * AverageuserFees * hourlyVolume;
            flips[item.ProductId] = new BazaarFlip
            {
                ItemTag = item.ProductId,
                BuyPrice = status.BuyPrice,
                SellPrice = status.SellPrice,
                ProfitPerHour = coinsPerHour,
                EstimatedFees = fees,
                Volume = volume,
                Timestamp = DateTime.UtcNow
            };
        }
        foreach (var item in flips.Values.OrderByDescending(v => v.ProfitPerHour - v.EstimatedFees).Take(100))
        {
            var history = await GetItemPriceHistory(item.ItemTag, DateTime.UtcNow.AddDays(-7));
            var medianBuyPrice = history.Select(h => h.Buy).OrderByDescending(b => b).ElementAt(history.Count / 2);
            item.MedianBuyPrice = medianBuyPrice;
        }
        logger.LogInformation($"Updated {update.Products.Count} flips");
        lastUpdate = update.Timestamp;
    }

    /// <summary>
    /// Returns all flips available
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    internal Task<List<BazaarFlip>> GetFlips()
    {
        return Task.FromResult(flips.Values.OrderByDescending(v => v.ProfitPerHour - v.EstimatedFees).ToList());
    }

    /// <summary>
    /// Returns the price history of an item.
    /// Longer time frames will return less data points (grouped together).
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    private async Task<List<Client.Model.GraphResult>> GetItemPriceHistory(string itemId, DateTime? start = null, DateTime? end = null)
    {
        return await client.ApiBazaarItemIdHistoryGetAsync(itemId, start, end);
    }

    public bool Ready()
    {
        return lastUpdate > DateTime.UtcNow.AddMinutes(-5);
    }
}