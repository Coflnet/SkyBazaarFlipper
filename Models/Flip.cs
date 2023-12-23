
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Coflnet.Sky.Bazaar.Flipper.Models;
[DataContract]
public class BazaarFlip
{
    public string ItemTag { get; set; }

    public double BuyPrice { get; set; }
    public double SellPrice { get; set; }
    public double ProfitPerHour { get; set; }
    public long Volume { get; set; }
    public DateTime Timestamp { get; set; }
    public double MedianBuyPrice { get; internal set; }
}