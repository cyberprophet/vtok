using DevExpress.Maui.Charts;

using Microsoft.AspNetCore.SignalR.Client;

using Newtonsoft.Json;

using ShareInvest.Infrastructure;
using ShareInvest.Mappers.Kiwoom;
using ShareInvest.Models;
using ShareInvest.Properties;
using ShareInvest.Services;

using System.Collections.ObjectModel;

namespace ShareInvest.ViewModels;

[QueryProperty(nameof(Code), nameof(Code)),
 QueryProperty(nameof(Name), nameof(Name))]
public partial class StockChartViewModel : ViewModelBase
{
    public override async Task DisposeAsync()
    {
        if (NetworkAccess.Internet == connectivity.NetworkAccess)
        {
            IsBusy = true;

            ChartCollection.Clear();

            await hub.StopAsync();

            GC.Collect();
        }
        IsBusy = false;
    }
    public override async Task InitializeAsync()
    {
        if (IsNotBusy)

            try
            {
                if (NetworkAccess.Internet == connectivity.NetworkAccess)
                {
                    IsBusy = true;

                    if (HubConnectionState.Disconnected == hub.State)
                    {
                        await hub.StartAsync();
                    }
                }
                else
                {
                    Title = nameof(connectivity.NetworkAccess);

                    await DisplayAlert(Resources.NETWORKACCESS);
                }
            }
            catch (Exception ex)
            {
                Title = ex.TargetSite?.Name;

                await DisplayAlert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
    }
    public async Task LoadChartAsync()
    {
        if (NetworkAccess.Internet != connectivity.NetworkAccess ||
            ChartCollection.Count > 0 ||
            string.IsNullOrEmpty(Code))
        {
#if DEBUG
            var json = JsonConvert.SerializeObject(new
            {
                Name,
                ChartCollection.Count,
                connectivity.NetworkAccess
            },
            Formatting.Indented);

            System.Diagnostics.Debug.WriteLine(json);
#endif
            return;
        }
        foreach (var dc in await service.GetAsync(EnumChartDuration.DailyChart, Code))
        {
            ChartCollection.Add(dc);
#if DEBUG
            var json = JsonConvert.SerializeObject(dc, Formatting.Indented);

            System.Diagnostics.Debug.WriteLine(json);
#endif
            Date = dc.Date;
        }
    }
    public string? Code
    {
        get; set;
    }
    public string? Name
    {
        get; set;
    }
    public ChartTheme ChartTheme
    {
        get
        {
#if DEBUG
            var json = JsonConvert.SerializeObject(new
            {
                user = Application.Current?.UserAppTheme,
                platform = Application.Current?.PlatformAppTheme,
                state = hub.State
            },
            Formatting.Indented);

            System.Diagnostics.Debug.WriteLine(json);
#endif
            return Application.Current?.PlatformAppTheme switch
            {
                AppTheme.Dark => ChartTheme.Dark,

                AppTheme.Light => ChartTheme.Light,

                _ => (ChartTheme)(DateTime.Now.Second % 2)
            };
        }
    }
    public ObservableCollection<ObservableStockStatus> ChartCollection
    {
        get;
    }
    public StockChartViewModel(IHubService hub,
                               IConnectivity connectivity,
                               StockService service)
    {
        this.hub = hub;
        this.connectivity = connectivity;
        this.service = service;

        if (hub is StockHubService sh)
        {
            sh.Send += (sender, e) =>
            {

            };
        }
        ChartCollection = new ObservableCollection<ObservableStockStatus>();
    }
    string? Date
    {
        get; set;
    }
    readonly StockService service;
    readonly IHubService hub;
    readonly IConnectivity connectivity;
}