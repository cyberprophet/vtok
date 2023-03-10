using System.Collections.Concurrent;

namespace ShareInvest.Server.Services;

public class StockService
{
    public ConcurrentDictionary<string, string> KiwoomUsers
    {
        get;
    }
    public ConcurrentDictionary<string, string> StocksConclusion
    {
        get;
    }
    public ConcurrentDictionary<string, int> RemainingQueue
    {
        get;
    }
    public string[] MarketOperation
    {
        get;
    }
    public StockService()
    {
        KiwoomUsers = new ConcurrentDictionary<string, string>();
        StocksConclusion = new ConcurrentDictionary<string, string>();
        RemainingQueue = new ConcurrentDictionary<string, int>();
        MarketOperation = new string[3];
    }
}