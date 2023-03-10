using ShareInvest.Identifies;
using ShareInvest.Infrastructure.Http;
using ShareInvest.Infrastructure.Socket;
using ShareInvest.Models.Google;
using ShareInvest.Properties;
using ShareInvest.Services;

using System.Diagnostics;

namespace ShareInvest;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
#if DEBUG
        args.ToList()
            .ForEach(arg => Debug.WriteLine(arg));

        Debug.WriteLine(Status.MacAddress);
#endif
        if (args.Length > 0 &&
            Status.GetKey(KeyDecoder.ProductKeyFromRegistry?.Split('-')) is string key)
        {
            Status.SetDebug();

            var client = new CoreRestClient(Status.Address);

            _ = Task.Run(async () =>
            {
                using (var api = new CoreMember(client))

                    if (await api.CheckOneIsMembershipAsync(key) is string membership)
                    {
                        var geo = new CoreRestClient(Resources.GOOGLE);

                        var request = new GeoRequest
                        {
                            WifiAccessPoints = new WifiAccessPoint[]
                            {
                                new WifiAccessPoint
                                {
                                    MacAddress = Status.MacAddress
                                }
                            },
                            ConsiderIp = string.IsNullOrEmpty(membership)
                        };
                        await api.AcceptNewMember(await geo.SetGeographicalLocation(membership,
                                                                                    request),
                                                  key);
                    }
            });
            ApplicationConfiguration.Initialize();

            Application.Run(new Securities(new[]
            {
                Resources.bird_idle,
                Resources.bird_awake,
                Resources.bird_alert,
                Resources.bird_sleep,
                Resources.bird_invisible
            },
            args,
            client,
            new CoreSignalR(key,
                            string.Concat(Status.Address,
                                          Resources.KIWOOM)),
            key));
        }
        GC.Collect();

        Process.GetCurrentProcess().Kill();
    }
}