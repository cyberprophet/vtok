using CommunityToolkit.Mvvm.Input;

using DevExpress.Maui.Scheduler.Internal;

using Microsoft.AspNetCore.SignalR.Client;

using ShareInvest.Infrastructure;
using ShareInvest.Mappers;
using ShareInvest.Models;
using ShareInvest.Observers.OpenAPI;
using ShareInvest.Pages;
using ShareInvest.Properties;
using ShareInvest.Services;

using System.Collections.ObjectModel;

namespace ShareInvest.ViewModels;

public partial class StocksViewModel : ViewModelBase
{
    public ObservableCollection<ObservableStock> StockCollection
    {
        get;
    }
    public override async Task DisposeAsync()
    {
        if (NetworkAccess.Internet == connectivity.NetworkAccess)
        {
            if (HubConnectionState.Connected == hub.State)
            {
                IsBusy = true;

                foreach (var stock in StockCollection)
                {
                    if (string.IsNullOrEmpty(stock.Code) is false)
                    {
                        await hub.RemoveFromGroupAsync(stock.Code);
                    }
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(stock.Name);
#endif
                }
                if (HubConnectionState.Connected == hub.State)
                {
                    await hub.StopAsync();
                }
            }
            IsBusy = false;
        }
#if DEBUG
        System.Diagnostics.Debug.WriteLine(hub.State);
#endif
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
#if DEBUG
                        System.Diagnostics.Debug.WriteLine(hub.State);
#endif
                        await hub.StartAsync();
                    }
                    if (HubConnectionState.Connected == hub.State)
                    {
                        foreach (var stock in StockCollection)

                            if (string.IsNullOrEmpty(stock.Code) is false)
                            {
#if DEBUG
                                System.Diagnostics.Debug.WriteLine(stock.Name);
#endif
                                await hub.AddToGroupAsync(stock.Code);
                            }
                        Stocks = await service.GetAsync(Title);
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
                Title = nameof(Exception);

                await DisplayAlert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
    }
    public async Task LoadStocksAsync()
    {
        if (IsLoading)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(IsLoading);
#endif
            return;
        }
        IsLoading = true;

        var length = (ChunkCount + 1) * chunkSize;

        while (HubConnectionState.Connected != hub.State ||
               Stocks is null)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(hub.State);
#endif
            await Task.Delay(0x400);
        }
        for (int i = StockCollection.Count; i < length; i++)
        {
            if (i < Stocks?.Length)
            {
                var stock = Stocks[i];

                if (string.IsNullOrEmpty(stock.Code) is false)
                {
                    await hub.AddToGroupAsync(stock.Code);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(stock.Name);
#endif
                    StockCollection.Add(stock);
                }
                continue;
            }
            break;
        }
        ChunkCount++;

        IsLoading = false;

        IsBusy = false;
    }
    public StocksViewModel(StockService service,
                           IHubService hub,
                           IPropertyService property,
                           IConnectivity connectivity)
    {
        chunkSize = 0x10;

        this.hub = hub;
        this.property = property;
        this.service = service;
        this.connectivity = connectivity;

        if (hub is StockHubService sh)
        {
            sh.Send += (sender, e) =>
            {
                if (e is RealMessageEventArgs res &&
                    Stocks != null)
                {
                    var index = Array.FindIndex(Stocks, o => res.Key.Equals(o.Code));

                    if (index >= 0 &&
                        StockCollection.TryGetValue(index,
                                                    out ObservableStock observe))
                    {
                        var resource = res.Data.Split('\t');

                        property.SetValuesOfColumn(observe,
                                                   resource.Length switch
                                                   {
                                                       7 => new ObservableStock(resource[1],
                                                                                resource[3],
                                                                                resource[2],
                                                                                resource[6],
                                                                                resource[5]),

                                                       _ => new ObservableStock(resource[1],
                                                                                resource[3],
                                                                                resource[2],
                                                                                resource[0xC],
                                                                                resource[7],
                                                                                resource[8])
                                                   });
                    }
                    return;
                }
            };
        }
        Title = nameof(Models.OpenAPI.Response.OPTKWFID.MarketCap);

        StockCollection = new ObservableCollection<ObservableStock>();
    }
    [RelayCommand]
    async Task SwipeLeft(ObservableStock stock)
    {
        await Shell.Current.GoToAsync(string.Concat(nameof(StockChartPage),
                                                    '?',
                                                    nameof(stock.Code),
                                                    '=',
                                                    stock.Code,
                                                    '&',
                                                    nameof(stock.Name),
                                                    '=',
                                                    stock.Name));
    }
    ObservableStock[]? Stocks
    {
        get; set;
    }
    bool IsLoading
    {
        get; set;
    }
    uint ChunkCount
    {
        get; set;
    }
    readonly uint chunkSize;
    readonly StockService service;
    readonly IPropertyService property;
    readonly IHubService hub;
    readonly IConnectivity connectivity;
}