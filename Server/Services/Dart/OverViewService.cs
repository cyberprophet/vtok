using Geocoding;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;

using ShareInvest.Infrastructure;
using ShareInvest.Infrastructure.Http;
using ShareInvest.Mappers;
using ShareInvest.Mappers.Kiwoom;
using ShareInvest.Models.Dart;
using ShareInvest.Server.Data;

using System.Diagnostics;
using System.Net;

namespace ShareInvest.Server.Services.Dart;

public class OverViewService : IScopedProcessingService
{
    public OverViewService(CoreContext context,
                           StockService stock,
                           ILogger<OverViewService> logger,
                           IPropertyService property,
                           IGeocoder geo)
    {
        this.geo = geo;
        this.stock = stock;
        this.logger = logger;
        this.context = context;
        this.property = property;

        cts = new CancellationTokenSource();
        api = new CoreRestClient(Properties.Resources.KAKAO);
    }
    public async Task DoWorkAsync(string key,
                                  string authorization,
                                  ICoreClient client,
                                  CancellationToken stoppingToken)
    {
        var emo = MarketOperation.Get(stock.MarketOperation[0]);

        if (EnumMarketOperation.장마감 == emo ||
            EnumMarketOperation.장시작전 == emo)
        {
            Authorization = authorization;
            API = client as CoreRestClient;
            Key = key;

            await foreach (var stock in GetCorpCodeAsync())
            {
                if (string.IsNullOrEmpty(stock.CorpCode) ||
                    string.IsNullOrEmpty(stock.Code))
                    continue;

                var company = await GetCompanyAsync(stock.CorpCode);

                if (company != null &&
                    context.Companies != null)
                {
                    var dao = context.Companies.AsNoTracking();

                    if (string.IsNullOrEmpty(company.Address) ||
                        IsUnique(dao, stock.Code, company.Address))
                        continue;

                    var address = await GeocodeAsync(company.Address, dao);

                    if (address != null)
                    {
                        company.Status = address.Provider;
                        company.Message = address.FormattedAddress;
                        company.Latitude = address.Coordinates.Latitude;
                        company.Longitude = address.Coordinates.Longitude;
                    }
                    foreach (var co in await GeocodeAsync(company.Address))
                    {
                        if (double.TryParse(co.Road.Latitude, out double latitude) &&
                            double.TryParse(co.Road.Longitude, out double longitude))
                        {
                            company.Latitude = latitude + 1e-4 * new Random().Next(-2, 3);
                            company.Longitude = longitude - 1e-4 * new Random().Next(-2, 3);
                            company.Status = Properties.Resources.AK[..^2];
                            company.Message = co.Road.Name;
                        }
                        if (string.IsNullOrEmpty(co.Road.BuildingName) is false)
                            break;
                    }
                    company.ModifyDate = stock.ModifyDate;
                    company.CorpCode = stock.CorpCode;
                    company.Date = DateTime.Now;

                    AddOrUpdate(company);
                }
                await Task.Delay(0x3C, stoppingToken);
            }
        }
    }
    bool IsUnique(IQueryable<CompanyOverview> dao,
                  string code,
                  string address)
    {
        var any = dao.Any(o => code.Equals(o.Code) &&
                               address.Equals(o.Address));

        if (any)
        {
            var unique = dao.Single(o => code.Equals(o.Code) &&
                                         address.Equals(o.Address));

            var empty = Properties.Resources.EMPTY.Equals(unique.Status);

            if (empty)
                return false;

            var count = dao.Count(o => o.Latitude == unique.Latitude &&
                                       o.Longitude == unique.Longitude);

            if (count == 1)
                logger.LogInformation("{ } is location is the only coordinate.",
                                      address);

            return count == 1;
        }
        logger.LogInformation("new company { } information is coming in.",
                              code);

        return any;
    }
    void AddOrUpdate(CompanyOverview company)
    {
        var tuple = context.Companies?.Find(company.Code);

        if (tuple != null)
        {
            property.SetValuesOfColumn(tuple, company);
        }
        else
        {
            context.Companies?.Add(company);
        }
        if (context.SaveChanges() > 0)
        {
#if DEBUG
            Debug.WriteLine(JsonConvert.SerializeObject(company,
                                                        Formatting.Indented));
#endif
        }
    }
    async Task<Address?> GeocodeAsync(string address, IQueryable<CompanyOverview> dao)
    {
        var geo = (await this.geo.GeocodeAsync(address))
                                 .FirstOrDefault(o => o.Coordinates.Longitude != 0 &&
                                                      o.Coordinates.Latitude != 0);

        while (geo != null &&
               geo.Coordinates.Longitude != 0 &&
               geo.Coordinates.Latitude != 0 &&
               dao.Count(o => o.Longitude == geo.Coordinates.Longitude &&
                              o.Latitude == geo.Coordinates.Latitude) > 1)
        {
            geo.Coordinates.Longitude += 1e-4 * new Random().Next(-2, 3);
            geo.Coordinates.Latitude -= 1e-4 * new Random().Next(-2, 3);
        }
        return geo;
    }
    async Task<Models.Kakao.Documents[]> GeocodeAsync(string address)
    {
        try
        {
            var request = new RestRequest(string.Concat(Properties.Resources.SEARCH,
                                                        address),
                                          Method.GET);

            request.AddHeader(nameof(Authorization),
                              Authorization ?? string.Empty);

            var res = await api.ExecuteAsync(request, cts.Token);

            if (HttpStatusCode.OK == res.StatusCode)
                return JsonConvert.DeserializeObject<Models.Kakao.LocalAddress>(res.Content).Document;
        }
        catch (Exception ex)
        {
            logger.LogWarning("{ } occurred while retrieving coordinates.",
                              ex.Message);
        }
        return Array.Empty<Models.Kakao.Documents>();
    }
    async Task<CompanyOverview?> GetCompanyAsync(string corpCode)
    {
        var route = string.Concat(nameof(api),
                                  '/',
                                  Properties.Resources.COMPANY);

        return API is null ? null :

               await API.GetCompanyOverviewAsync(route,
                                                 JToken.FromObject(new
                                                 {
                                                     crtfc_key = Key,
                                                     corp_code = corpCode
                                                 }));
    }
    async IAsyncEnumerable<DartCode> GetCorpCodeAsync()
    {
        var route = string.Concat(nameof(api),
                                  '/',
                                  Properties.Resources.CORPCODE);

        if (API != null)
        {
            foreach (var stock in await API.GetCorpCodeAsync(route,
                                                             JToken.FromObject(new
                                                             {
                                                                 crtfc_key = Key
                                                             })))
                yield return stock;
        }
    }
    string? Authorization
    {
        get; set;
    }
    string? Key
    {
        get; set;
    }
    CoreRestClient? API
    {
        get; set;
    }
    readonly StockService stock;
    readonly CancellationTokenSource cts;
    readonly CoreRestClient api;
    readonly CoreContext context;
    readonly ILogger<OverViewService> logger;
    readonly IPropertyService property;
    readonly IGeocoder geo;
}