using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;

using ShareInvest.Models.Dart;
using ShareInvest.Models.Google;
using ShareInvest.Properties;

using System.IO.Compression;
using System.Net;
using System.Text;
using System.Xml;

namespace ShareInvest.Infrastructure.Http;

public class CoreRestClient : RestClient, ICoreClient
{
    public async Task<string> PostAsync<T>(int index, string route, T param) where T : class
    {
        string transformer = Identifies.Parameter.TransformOutbound(route),
               resource = string.Concat(routes[index], '/', transformer);

        var request = new RestRequest(resource, Method.POST);

        request.AddJsonBody(param);

        var res = await ExecuteAsync(request,
                                     cancellationTokenSource.Token);

        if (HttpStatusCode.OK != res.StatusCode)
        {
            Status.WriteLine(resource, res);
        }
        return res.Content;
    }
    public async Task<object> PostAsync<T>(string route, T param) where T : class
    {
        string transformer = Identifies.Parameter.TransformOutbound(route),
               resource = string.Concat(routes[0], '/', transformer);

        var request = new RestRequest(resource, Method.POST);

        request.AddJsonBody(param);

        var res = await ExecuteAsync(request,
                                     cancellationTokenSource.Token);

        if (HttpStatusCode.OK != res.StatusCode)
        {
            Status.WriteLine(resource, res);
        }
        return res.Content;
    }
    public async Task<GeoResponse> SetGeographicalLocation(string key, GeoRequest geo)
    {
        var request = new RestRequest(string.Concat(Resources.GEOLOCATE, key),
                                      Method.POST);

        var res = await ExecuteAsync(request.AddJsonBody(geo),
                                     cancellationTokenSource.Token);

        var geoRes = JsonConvert.DeserializeObject<GeoResponse>(res.Content);
#if DEBUG
        var json = JsonConvert.SerializeObject(geoRes, Newtonsoft.Json.Formatting.Indented);

        System.Diagnostics.Debug.WriteLine(json, res.StatusDescription);
#endif
        return geoRes;
    }
    public async Task<string> GetUserAuthAsync(JToken token)
    {
        var resource = Identifies.Parameter.TransformQuery(token,
                                                           new StringBuilder(Resources.AUTH));

        var res = await ExecuteAsync(new RestRequest(resource, Method.GET),
                                     cancellationTokenSource.Token);

        if (HttpStatusCode.OK != res.StatusCode)
        {
            Status.WriteLine(resource, res);
        }
        return res.Content;
    }
    public async Task<IEnumerable<DartCode>> GetCorpCodeAsync(string route, JToken token)
    {
        var query = Identifies.Parameter.TransformQuery(token);

        var res = await ExecuteAsync(new RestRequest(string.Concat(route, query),
                                                     Method.GET),
                                     cancellationTokenSource.Token);

        using (var stream = new MemoryStream(res.RawBytes))
        {
            var list = new Stack<DartCode>();

            using (var compress = new ZipArchive(stream, ZipArchiveMode.Read))

                foreach (var entry in compress.Entries)

                    using (var sr = new StreamReader(entry.Open()))
                    {
                        var xml = new XmlDocument();

                        xml.LoadXml(sr.ReadToEnd());

                        foreach (XmlNode node in xml.GetElementsByTagName(nameof(list)))

                            if (string.IsNullOrEmpty(node[Resources.STOCKCODE]?.InnerText))
                                continue;

                            else
                                list.Push(new DartCode
                                {
                                    Code = node[Resources.STOCKCODE]?.InnerText,
                                    CorpCode = node[Resources.CORPCODE]?.InnerText,
                                    CorpName = node[Resources.NAME]?.InnerText,
                                    ModifyDate = node[Resources.MODIFY]?.InnerText
                                });
                    }
            return list;
        }
    }
    public async Task<CompanyOverview?> GetCompanyOverviewAsync(string route, JToken token)
    {
        var query = Identifies.Parameter.TransformQuery(token);

        var res = await ExecuteAsync(new RestRequest(string.Concat(route, query),
                                                     Method.GET),
                                     cancellationTokenSource.Token);

        return HttpStatusCode.OK == res.StatusCode ?

            JsonConvert.DeserializeObject<CompanyOverview>(res.Content) : null;
    }
    public async Task<object> GetAsync(string route)
    {
        var resource = string.Concat(routes[0],
                                     '/',
                                     Identifies.Parameter.TransformOutbound(route));

        var res = await ExecuteAsync(new RestRequest(resource, Method.GET),
                                     cancellationTokenSource.Token);

        if (HttpStatusCode.OK != res.StatusCode)
        {
            Status.WriteLine(resource, res);
        }
        return res.Content;
    }
    public async Task<object> GetAsync(string route, JToken token)
    {
        var resource = string.Concat(routes[0],
                                     '/',
                                     Identifies.Parameter.TransformQuery(token,
                                                                         new StringBuilder(route)));

        var res = await ExecuteAsync(new RestRequest(resource,
                                                     Method.GET),
                                     cancellationTokenSource.Token);

        if (HttpStatusCode.OK != res.StatusCode)
        {
            Status.WriteLine(resource, res);
        }
        return res.Content;
    }
    public CoreRestClient() : base(Resources.URL)
    {
        routes = new[]
        {
            Resources.KIWOOM,
            Resources.CORE
        };
        Timeout = -1;
        cancellationTokenSource = new CancellationTokenSource();
    }
    public CoreRestClient(string url) : base(url)
    {
        routes = new[]
        {
            Resources.KIWOOM,
            Resources.CORE
        };
        Timeout = -1;
        cancellationTokenSource = new CancellationTokenSource();
    }
    readonly string[] routes;
    readonly CancellationTokenSource cancellationTokenSource;
}