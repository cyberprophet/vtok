using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using ShareInvest.Models;
using ShareInvest.Models.Charts;
using ShareInvest.Observers;
using ShareInvest.Observers.OpenAPI;
using ShareInvest.Observers.Socket;
using ShareInvest.Properties;

using System.Diagnostics;

namespace ShareInvest.Infrastructure.Socket;

public class CoreSignalR : ISocketClient<MessageEventArgs>
{
    public event EventHandler<MessageEventArgs>? Send;

    public HubConnection Hub
    {
        get;
    }
    public CoreSignalR(string key, string url)
    {
        Hub = new HubConnectionBuilder()

            .WithUrl(url, o =>
            {
                o.AccessTokenProvider = async () =>
                {


                    return await Task.FromResult(string.Empty);
                };
                _ = o.Headers.TryAdd(Resources.SECURITY, key);
            })
            .AddNewtonsoftJsonProtocol(o =>
            {
                o.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            })
            .ConfigureLogging(o =>
            {
                o.AddDebug();
                o.SetMinimumLevel(LogLevel.Debug);
            })
            .WithAutomaticReconnect(new TimeSpan[]
            {
                TimeSpan.Zero,
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(0xA),
                TimeSpan.FromSeconds(0x20),
                TimeSpan.FromSeconds(0x5A)
            })
            .Build();

        OnConnection();
    }
    public CoreSignalR(string url)
    {
        Hub = new HubConnectionBuilder()

            .WithUrl(url, o =>
            {
                o.AccessTokenProvider = async () =>
                {


                    return await Task.FromResult(string.Empty);
                };
            })
            .AddNewtonsoftJsonProtocol(o =>
            {
                o.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            })
            .ConfigureLogging(o =>
            {
                o.AddDebug();
                o.SetMinimumLevel(LogLevel.Debug);
            })
            .WithAutomaticReconnect(new TimeSpan[]
            {
                TimeSpan.Zero,
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(0xA),
                TimeSpan.FromSeconds(0x20),
                TimeSpan.FromSeconds(0x5A)
            })
            .Build();

        OnConnection();
    }
    public IDisposable On(string name)
    {
#if DEBUG
        Debug.WriteLine(name);
#endif
        return OnActions[name];
    }
    Dictionary<string, IDisposable> OnActions => new()
    {
        {
            nameof(IHubs.UpdateTheStatusOfBalances),
            Hub.On<Balance>(nameof(IHubs.UpdateTheStatusOfBalances),
                            bal => Send?.Invoke(this, new InstructEventArgs(bal)))
        },
        {
            nameof(IHubs.UpdateTheStatusOfAssets),
            Hub.On<Account>(nameof(IHubs.UpdateTheStatusOfAssets),
                            acc => Send?.Invoke(this, new InstructEventArgs(acc)))
        },
        {
            nameof(IHubs.AddToGroupAsync),
            Hub.On<string>(nameof(IHubs.AddToGroupAsync),
                           groupName => Send?.Invoke(this, new GroupEventArgs(groupName)))
        },
        {
            nameof(IHubs.TransmitConclusionInformation),
            Hub.On<string, string>(nameof(IHubs.TransmitConclusionInformation),
                                  (key, data) => Send?.Invoke(this, new RealMessageEventArgs(key, data)))
        },
        {
            nameof(IHubs.InstructToRenewAssetStatus),
            Hub.On<string>(nameof(IHubs.InstructToRenewAssetStatus),
                           acc => Send?.Invoke(this, new InstructEventArgs(acc)))
        },
        {
            nameof(IHubs.GetAssetStatusByDate),
            Hub.On<IEnumerable<AssetStatus>>(nameof(IHubs.GetAssetStatusByDate),
                                             handler =>
                                             {
                                                 foreach(var status in handler)

                                                    Send?.Invoke(this, new InstructEventArgs(status));
                                             })
        }
    };
    void OnConnection()
    {
        Hub.Closed += async e =>
        {
            Send?.Invoke(this, new SignalEventArgs(Hub.State));
#if DEBUG
            Debug.WriteLine(e?.Message);
#endif
            await Task.CompletedTask;
        };
        Hub.Reconnecting += async e =>
        {
            Send?.Invoke(this, new SignalEventArgs(Hub.State));
#if DEBUG
            Debug.WriteLine(e?.Message);
#endif
            await Task.CompletedTask;
        };
        Hub.ServerTimeout = TimeSpan.FromSeconds(60);
        Hub.HandshakeTimeout = TimeSpan.FromSeconds(30);
    }
}