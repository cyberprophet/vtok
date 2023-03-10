using Newtonsoft.Json;

using RestSharp;

using ShareInvest.Infrastructure.Http;
using ShareInvest.Properties;

using System.Media;

namespace ShareInvest.Services;

class CoreMember : IDisposable
{
    public void Dispose()
    {
        source.Dispose();
    }
    internal async Task<string?> CheckOneIsMembershipAsync(string key)
    {
        var route = string.Concat(Resources.API,
                                  '/',
                                  nameof(CoreMember),
                                  '?',
                                  nameof(key),
                                  '=',
                                  key);

        var resource = Identifies.Parameter.TransformOutbound(route);

        var res = await api.ExecuteAsync(new RestRequest(resource, Method.GET),
                                         source.Token);

        var auth = JsonConvert.DeserializeObject<Models.Google.Authorization>(res.Content);

        if (string.IsNullOrEmpty(auth?.Key))

            using (var sp = new SoundPlayer(Resources.EXISTENCE))
            {
                sp.PlaySync();

                return null;
            }
        return auth?.Key;
    }
    internal async Task AcceptNewMember(Models.Google.GeoResponse geo, string key)
    {
        var route = string.Concat(Resources.API, '/', nameof(CoreMember));

        var resource = Identifies.Parameter.TransformOutbound(route);

        var request = new RestRequest(resource, Method.POST);

        request.AddJsonBody(new Models.Member
        {
            Key = key,
            Accuracy = geo.Accuracy,
            Latitude = geo.Location.Latitude,
            Longitude = geo.Location.Longitude
        });
        var res = await api.ExecuteAsync(request, source.Token);

#if DEBUG
        Status.WriteLine(resource, res);
#endif
    }
    internal CoreMember(CoreRestClient api)
    {
        this.api = api;

        source = new CancellationTokenSource();
    }
    readonly CoreRestClient api;
    readonly CancellationTokenSource source;
}