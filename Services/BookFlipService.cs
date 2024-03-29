using System.Threading.Tasks;
using Coflnet.Sky.Bazaar.Flipper.Models;
using System;
using System.Linq;
using dev;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;

namespace Coflnet.Sky.Bazaar.Flipper.Services;

public class BookFlipService
{
    private const double AverageuserFees = 0.0125;

    private Dictionary<string, BookFlip> bookFlips = new Dictionary<string, BookFlip>();
    private HashSet<string> combineableBooks = new HashSet<string>();
    private ILogger<BookFlipService> logger;

    public BookFlipService(ILogger<BookFlipService> logger)
    {
        this.logger = logger;
        var bookCrafts = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, string[]>>>(File.ReadAllText("Constants/enchants.json"));
        foreach (var enchant in bookCrafts)
        {
            foreach (var item in enchant.Value)
            {
                (var level, var sources) = item;
                if(!sources.Any(s=>s.Contains("Combining")))
                {
                    continue;
                }
                var id = $"ENCHANTMENT_{enchant.Key.Replace(' ', '_').ToUpper()}_{level}";
                logger.LogInformation($"Added {id}");
                combineableBooks.Add(id);
            }
        }
    }

    internal async Task BookUpdate(BazaarPull update)
    {
        var groups = update.Products.Where(p => p.ProductId.StartsWith("ENCHANTMENT_") && p.QuickStatus.BuyMovingWeek > 0 && p.QuickStatus.SellMovingWeek > 0)
                .Select(p => new
                {
                    // from ENCHANTMENT_ULTIMATE_NO_PAIN_NO_GAIN_1 get 1 as level and NO_PAIN_NO_GAIN as enchantment
                    Level = int.Parse(p.ProductId.Split('_').Last()),
                    Enchantment = p.ProductId.Split('_').Skip(1).TakeWhile(s => s.All(char.IsUpper)).Aggregate((a, b) => a + "_" + b),
                    Details = p
                }
                ).GroupBy(p => p.Enchantment);
        foreach (var item in groups)
        {
            if (item.Count() < 2)
            {
                continue;
            }
            var ordered = item.OrderBy(i => i.Level).ToList();
            var previous = ordered.First();
            for (int i = 1; i < ordered.Count; i++)
            {
                var current = ordered[i];
                if (!combineableBooks.Contains(current.Details.ProductId))
                    continue;
                var profit = current.Details.QuickStatus.SellPrice - previous.Details.QuickStatus.BuyPrice;
                var volume = Math.Min(previous.Details.QuickStatus.BuyMovingWeek, previous.Details.QuickStatus.SellMovingWeek);
                bookFlips[current.Details.ProductId] = new BookFlip
                {
                    StartTag = previous.Details.ProductId,
                    EndTag = current.Details.ProductId,
                    ProfitPerHour = profit / 168,
                    HourlyVolume = volume,
                    EstimatedFees = previous.Details.QuickStatus.BuyPrice * AverageuserFees * volume / 168,
                    Timestamp = DateTime.UtcNow
                };
                previous = current;
            }

        }
        logger.LogInformation($"Updated {groups.Select(g => g.Count()).Sum()} books");
        logger.LogInformation($"Enchant count {groups.Count()}");
    }

    internal async Task<List<BookFlip>> GetFlips()
    {
        return bookFlips.Values.OrderByDescending(b => b.ProfitPerHour).Take(80).ToList();
    }
}