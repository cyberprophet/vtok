using ShareInvest.Infrastructure.Http;
using ShareInvest.Mappers.Kiwoom;
using ShareInvest.Models;
using ShareInvest.Models.OpenAPI.Response;

namespace ShareInvest.Services;

public class StockService : CoreHttpClient
{
    public async Task<ObservableStock[]> GetAsync(string? order, bool asc = false)
    {
        var res = await TryGetAsync<Stock[]>(0,
                                             string.Concat(nameof(OPTKWFID),
                                                           '?',
                                                           nameof(order),
                                                           '=',
                                                           order,
                                                           '&',
                                                           nameof(asc),
                                                           '=',
                                                           asc));

        var arr = res?.Select(o => new ObservableStock(o.Code,
                                                       o.Name,
                                                       o.Current,
                                                       o.Rate,
                                                       o.CompareToPreviousDay,
                                                       o.CompareToPreviousSign,
                                                       o.Volume,
                                                       o.TransactionAmount,
                                                       o.State))
                      .ToArray();

        return arr ?? Array.Empty<ObservableStock>();
    }
    public async Task<ObservableStockStatus[]> GetAsync(EnumChartDuration duration,
                                                        string code,
                                                        string date = "",
                                                        int period = 0x100)
    {
        var query = string.Concat(nameof(OPT10081),
                                  '/',
                                  duration,
                                  '?',
                                  nameof(code),
                                  '=',
                                  code,
                                  '&',
                                  nameof(date),
                                  '=',
                                  date,
                                  '&',
                                  nameof(period),
                                  '=',
                                  period);
#if DEBUG
        System.Diagnostics.Debug.WriteLine(query);
#endif
        var res = await TryGetAsync<Chart[]>(0, query);

        var arr = res?.Select(s => new ObservableStockStatus(s.Code,
                                                             s.Date,
                                                             s.Current,
                                                             s.Start,
                                                             s.High,
                                                             s.Low,
                                                             s.Volume,
                                                             s.Name))
                      .ToArray();

        return arr ?? Array.Empty<ObservableStockStatus>();
    }
    public StockService() : base(Status.Address)
    {

    }
}