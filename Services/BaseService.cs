using System.Threading.Tasks;
using Coflnet.Sky.Bazaar.Flipper.Models;
using System;
using System.Linq;
using dev;
using Coflnet.Sky.Bazaar.Client.Api;
using System.Collections.Generic;

namespace Coflnet.Sky.Bazaar.Flipper.Services;
public class BazaarFlipperService
{
    private IBazaarApi client;

    public BazaarFlipperService(IBazaarApi client)
    {
        this.client = client;
    }

    /// <summary>
    /// Gets called whenever new bazaar data is available
    /// Should finish within 10 seconds to be called with the next bazaar update
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    internal Task BazaarUpdate(BazaarPull update)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all flips available
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    internal Task<Flip> GetFlips()
    {
        throw new NotImplementedException();
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
}