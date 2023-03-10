using DevExpress.Maui.Scheduler.Internal;

using Microsoft.AspNetCore.SignalR.Client;

using ShareInvest.Infrastructure;
using ShareInvest.Mappers;
using ShareInvest.Models;
using ShareInvest.Models.Charts;
using ShareInvest.Observers.Socket;
using ShareInvest.Properties;
using ShareInvest.Services;

using System.Collections.ObjectModel;

namespace ShareInvest.ViewModels;

[QueryProperty(nameof(AccNo), nameof(AccNo)),
 QueryProperty(nameof(Account), nameof(Account))]
public partial class AssetChartViewModel : ViewModelBase
{
    public ObservableCollection<ObservableAssetStatus> ChartCollection
    {
        get;
    }
    public AssetChartViewModel(IHubService hub,
                               IPropertyService property,
                               IConnectivity connectivity)
    {
        this.hub = hub;
        this.property = property;
        this.connectivity = connectivity;

        if (hub is StockHubService sh)
        {
            sh.Send += (sender, e) =>
            {
                if (e is not InstructEventArgs arg)
                {
                    return;
                }
                if (arg.Convey is not AssetStatus status)
                {
                    return;
                }
#if DEBUG
                Status.GetProperites(status);
#endif
                var index = ChartCollection.FindIndex(p => string.IsNullOrEmpty(status.Date) is false &&
                                                           status.Date.Equals(p.Date) &&
                                                           string.IsNullOrEmpty(status.AccNo) is false &&
                                                           status.AccNo.Equals(p.AccNo));

                if (ChartCollection.TryGetValue(index, out var asset))
                {

                }
                else
                    ChartCollection?.Add(new ObservableAssetStatus(status));
            };
        }
        ChartCollection = new ObservableCollection<ObservableAssetStatus>();
    }
    public override async Task DisposeAsync()
    {
        if (NetworkAccess.Internet == connectivity.NetworkAccess)
        {
            IsBusy = true;

            await hub.StopAsync();
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
                    if (HubConnectionState.Connected == hub.State &&
                        string.IsNullOrEmpty(AccNo) is false &&
                        ChartCollection.Count == 0 &&
                        hub is StockHubService sh)
                    {
                        await sh.RequestAssetStatusByDate(AccNo);
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
    public string? AccNo
    {
        get; set;
    }
    public string? Account
    {
        get; set;
    }
    readonly IHubService hub;
    readonly IConnectivity connectivity;
    readonly IPropertyService property;
}