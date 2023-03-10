using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

using ShareInvest.Infrastructure;
using ShareInvest.Mappers.Kiwoom;
using ShareInvest.Models.Charts;
using ShareInvest.Server.Data;
using ShareInvest.Server.Services;

namespace ShareInvest.Server.Hubs;

public class KiwoomHub : Hub<IHubs>
{
    public KiwoomHub(ILogger<KiwoomHub> logger,
                     StockService service,
                     CoreContext context)
    {
        this.logger = logger;
        this.service = service;
        this.context = context;
    }
    public async Task RequestAssetStatusByDate(string accNo)
    {
        if (context.Accounts != null && context.Balances != null)
        {
            var lookup = long.MaxValue;

            do
            {
                try
                {
                    var queryable = (from a in context.Accounts.AsNoTracking()
                                     where accNo.Equals(a.AccNo) && lookup > a.Lookup
                                     orderby a.Lookup descending
                                     select new AssetStatus
                                     {
                                         PresumeDeposit = a.PresumeDeposit,
                                         PresumeAsset = a.PresumeAsset,
                                         Lookup = a.Lookup,
                                         AccNo = a.AccNo,
                                         Date = a.Date,
                                         Holdings = (from b in context.Balances.AsNoTracking()

                                                     where string.IsNullOrEmpty(a.AccNo) == false &&
                                                           a.AccNo.Equals(b.AccNo) &&
                                                           string.IsNullOrEmpty(a.Date) == false &&
                                                           a.Date.Equals(b.Date)

                                                     select new Holdings(b)).ToArray()
                                     })
                                     .Take(0x40);

                    await Clients.Caller.GetAssetStatusByDate<IQueryable<AssetStatus>>(queryable);

                    if (queryable.Count() < 0x40)
                    {
                        logger.LogInformation("{ } check the asset status of account.", accNo);

                        break;
                    }
                    lookup = queryable.Min(selector => selector.Lookup);
                }
                catch (Exception ex)
                {
                    logger.LogError("occurs in { } inquiries.\n{ }", lookup, ex.Message);

                    lookup = long.MinValue;
                }
            }
            while (lookup > 0);
        }
    }
    public async Task InstructToRenewAssetStatus(string accNo)
    {
        if (context.KiwoomUsers != null)

            foreach (var user in from o in context.KiwoomUsers.AsNoTracking()
                                 where accNo.Equals(o.AccNo)
                                 select o.Key)

                if (service.KiwoomUsers.TryGetValue(user,
                                                    out string? connectionId))
                {
                    await Clients.Client(connectionId)
                                 .InstructToRenewAssetStatus(accNo);

                    await Groups.AddToGroupAsync(Context.ConnectionId, user);

                    await Clients.Caller.AddToGroupAsync(user);

                    logger.LogInformation("{ } joins the { }",
                                          Context.ConnectionId,
                                          user);
                }
    }
    public Task GatherCluesToPrioritize(int count)
    {
        if (service.RemainingQueue.TryGetValue(Context.ConnectionId,
                                               out int queue) &&
            queue != count)
        {
            service.RemainingQueue[Context.ConnectionId] = count;

            logger.LogInformation("remaining queue for { } is { }.",
                                  Context.ConnectionId,
                                  count);
        }
        return Task.CompletedTask;
    }
    public override async Task OnConnectedAsync()
    {
        var headers = Context.GetHttpContext()?.Request.Headers;

        if (headers != null &&
            headers.TryGetValue(Properties.Resources.SECURITY,
                                out StringValues value))
        {
            logger.LogWarning("[{ }] { } has joined the kiwoom.",
                              service.KiwoomUsers.TryAdd(value,
                                                         Context.ConnectionId) &&

                              service.RemainingQueue.TryAdd(Context.ConnectionId,
                                                            int.MaxValue),
                              value);
        }
        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var headers = Context.GetHttpContext()?.Request.Headers;

        if (headers != null &&
            headers.TryGetValue(Properties.Resources.SECURITY,
                                out StringValues value))
        {
            logger.LogWarning("[{ }] { } has left the kiwoom.",
                              service.KiwoomUsers.TryRemove(value, out string? id) &&
                              service.RemainingQueue.TryRemove(id, out int _),
                              value);
        }
        await base.OnDisconnectedAsync(exception);
    }
    public async Task AddToGroupAsync(string id, string code)
    {
        logger.LogInformation("{ } has joined the group { }.",
                              id,
                              code);

        await Groups.AddToGroupAsync(id, code);
    }
    public async Task RemoveFromGroupAsync(string id, string code)
    {
        logger.LogInformation("{ } has left the group { }.",
                              id,
                              code);

        await Groups.RemoveFromGroupAsync(id, code);
    }
    [HubMethodName("주식시세")]
    public void RealTypeMarketPrice(string key, string data)
    {

    }
    [HubMethodName("주식체결")]
    public void RealTypeSignStatus(string key, string data)
    {
        Clients.Group(key)
               .TransmitConclusionInformation(key, data);

        service.StocksConclusion[key] = data;
    }
    [HubMethodName("주식우선호가")]
    public void RealTypeFirstOfPrice(string key, string data)
    {

    }
    [HubMethodName("주식호가잔량")]
    public void RealTypeResidualQuantity(string key, string data)
    {

    }
    [HubMethodName("주식시간외호가")]
    public void RealTypeExtraHourPrice(string key, string data)
    {

    }
    [HubMethodName("주식당일거래원")]
    public void RealTypeMessage(string key, string data)
    {

    }
    [HubMethodName("ETF NAV")]
    public void RealTypeNAV(string key, string data)
    {

    }
    [HubMethodName("ELW 지표")]
    public void RealTypeIndicators(string key, string data)
    {

    }
    [HubMethodName("ELW 이론가")]
    public void RealTypeTheoreticalPrice(string key, string data)
    {

    }
    [HubMethodName("주식예상체결")]
    public void RealTypeEstimatedPrice(string key, string data)
    {
        Clients.Group(key)
               .TransmitConclusionInformation(key, data);

        service.StocksConclusion[key] = data;
    }
    [HubMethodName("주식종목정보")]
    public void RealTypeStockInformation(string key, string data)
    {
        logger.LogInformation("Stock Information Key is { }.\nData is { }.",
                              key,
                              data);
    }
    [HubMethodName("선물옵션우선호가")]
    public void RealTypePriorityPrice(string key, string data)
    {

    }
    [HubMethodName("선물시세")]
    public void RealTypeFuturesMarketPrice(string key, string data)
    {

    }
    [HubMethodName("선물호가잔량")]
    public void RealTypeFuturesResidualQuantity(string key, string data)
    {

    }
    [HubMethodName("선물이론가")]
    public void RealTypeFuturesTheoreticalPrice(string key, string data)
    {

    }
    [HubMethodName("옵션시세")]
    public void RealTypeOptionsMarketPrice(string key, string data)
    {

    }
    [HubMethodName("옵션호가잔량")]
    public void RealTypeOptionsResidualQuantity(string key, string data)
    {

    }
    [HubMethodName("옵션이론가")]
    public void RealTypeOptionsTheoreticalPrice(string key, string data)
    {

    }
    [HubMethodName("업종지수")]
    public void RealTypeIndex(string key, string data)
    {

    }
    [HubMethodName("업종등락")]
    public void RealTypeRate(string key, string data)
    {

    }
    [HubMethodName("장시작시간")]
    public void RealTypeOperation(string key, string data)
    {
        var operation = data.Split('\t');

        if (EnumMarketOperation.장마감 == MarketOperation.Get(operation[0]) &&
            context.KiwoomUsers != null)

            foreach (var user in from o in context.KiwoomUsers.AsNoTracking()
                                 where string.IsNullOrEmpty(o.AccNo) == false &&
                                       o.AccNo.Substring(o.AccNo.Length - 2).CompareTo("31") < 0
                                 select new
                                 {
                                     o.Key,
                                     o.AccNo
                                 })
            {
                if (service.KiwoomUsers.TryGetValue(user.Key, out string? connectionId))

                    Clients.Client(connectionId)
                           .InstructToRenewAssetStatus(user.AccNo);
            }
        for (int i = 0; i < service.MarketOperation.Length; i++)
        {
            service.MarketOperation[i] = operation[i];
        }
        logger.LogInformation("Market Operation Key is { }.\nData is { }.",
                              key,
                              data);
    }
    [HubMethodName("VI발동/해제")]
    public void RealTypeVolatilityInterruption(string key, string data)
    {

    }
    [HubMethodName("주문체결")]
    public void RealTypeConclusion(string key, string data)
    {

    }
    [HubMethodName("파생잔고")]
    public void RealTypeFuturesBalances(string key, string data)
    {

    }
    [HubMethodName("잔고")]
    public void RealTypeBalances(string key, string data)
    {

    }
    [HubMethodName("종목프로그램매매")]
    public void RealTypeProgramTrading(string key, string data)
    {

    }
    readonly ILogger<KiwoomHub> logger;
    readonly StockService service;
    readonly CoreContext context;
}