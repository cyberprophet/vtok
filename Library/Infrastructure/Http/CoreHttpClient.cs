using Newtonsoft.Json.Linq;

using ShareInvest.Identifies;
using ShareInvest.Properties;

using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace ShareInvest.Infrastructure.Http;

public class CoreHttpClient : HttpClient, ICoreClient
{
    public Task<object> GetAsync(string route, JToken token)
    {
        throw new NotImplementedException();
    }
    public Task<string> PostAsync<T>(int index, string route, T param) where T : class
    {
        throw new NotImplementedException();
    }
    public async Task<object> PostAsync<T>(string route, T param) where T : class
    {
        var url = string.Concat(Resources.URL, routes[0], route);

        using (var res = await this.PostAsJsonAsync(url, param, cts.Token))
        {
            if (HttpStatusCode.OK != res.StatusCode)
            {
#if DEBUG
                Debug.WriteLine(res.StatusCode);
#endif
            }
            else
                return res.Content;
        }
        return string.Empty;
    }
    public CoreHttpClient(string url)
    {
        routes = new[]
        {
            Resources.KIWOOM,
            Resources.CORE
        };
        FirstRendering = true;
        BaseAddress = new Uri(url);
    }
    protected Task<T?> TryGetAsync<T>(int routeIndex,
                                      string param)
    {
        string path = string.Concat(routes[routeIndex],
                                    '/',
                                    Parameter.TransformOutbound(param));
        if (FirstRendering)
        {
            FirstRendering = false;

            return Task.Run(() => TryGetImplementationAsync<T>(path));
        }
        return TryGetImplementationAsync<T>(path);
    }
    async Task<T?> TryGetImplementationAsync<T>(string path)
    {
        return await this.GetFromJsonAsync<T>(path);
    }
    Task<object> ICoreClient.GetAsync(string route)
    {
        throw new NotImplementedException();
    }
    bool FirstRendering
    {
        get; set;
    }
    readonly string[] routes;
    readonly CancellationTokenSource cts = new();
}