using CommunityToolkit.Mvvm.Input;

using DevExpress.Maui.Scheduler.Internal;

using Microsoft.AspNetCore.SignalR.Client;

using ShareInvest.Infrastructure;
using ShareInvest.Mappers;
using ShareInvest.Models;
using ShareInvest.Observers.Socket;
using ShareInvest.Pages;
using ShareInvest.Properties;
using ShareInvest.Services;

using System.Collections.ObjectModel;

namespace ShareInvest.ViewModels;

public partial class AccountsViewModel : ViewModelBase
{
    public ObservableCollection<ObservableAccount> AccountCollection
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

                while (groupNames.TryPop(out string? groupName))
                {
                    await hub.RemoveFromGroupAsync(groupName);
                }
                await hub.StopAsync();
            }
            accounts.Clear();
            AccountCollection.Clear();
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

                    var value = await SecureStorage.Default.GetAsync(Resources.AUTH);

                    if (HubConnectionState.Disconnected == hub.State)
                    {
                        await hub.StartAsync();
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        await SecureStorage.Default.SetAsync(Resources.AUTH,
                                                             Resources.KAKAO);
                    }
                    await foreach (var acc in login.AuthenticateAsync(value))
                    {
                        if (hub is StockHubService sh)
                        {
                            await sh.InstructToRenewAssetStatus(acc.AccNo);
                        }
                        AccountCollection.Add(acc);
                    }
                    if (AccountCollection.Count == 0)
                    {

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

                SecureStorage.Default.RemoveAll();
            }
            finally
            {
                IsBusy = false;
            }
    }
    public AccountsViewModel(IConnectivity connectivity,
                             ILoginService<ObservableAccount> login,
                             IHubService hub,
                             IPropertyService property)
    {
        this.connectivity = connectivity;
        this.property = property;
        this.login = login;
        this.hub = hub;

        if (hub is StockHubService sh)
        {
            sh.Send += (sender, e) =>
            {
                switch (e)
                {
                    case InstructEventArgs asset:

                        var index = AccountCollection.FindIndex(o => asset.AccNo.Equals(o.AccNo) &&
                                                                     asset.Date.Equals(o.Date));

                        switch (asset.Convey)
                        {
                            case Balance bal:

                                if (AccountCollection.TryGetValue(index, out ObservableAccount ob) &&
                                    ob.Balances != null)
                                {
                                    var tuple = Array.Find(ob.Balances,
                                                           match => string.IsNullOrEmpty(bal.Code) is false &&
                                                                    bal.Code.Equals(match.Code));

                                    var balance = new ObservableBalance(bal.Code,
                                                                        bal.Name,
                                                                        bal.AccNo,
                                                                        bal.Quantity,
                                                                        bal.Average,
                                                                        bal.Current,
                                                                        bal.Valuation,
                                                                        bal.Amount,
                                                                        bal.Rate,
                                                                        bal.Purchase,
                                                                        bal.PaymentBalance,
                                                                        bal.PreviousPurchaseQuantity,
                                                                        bal.PreviousSalesQuantity,
                                                                        bal.PurchaseQuantity,
                                                                        bal.SalesQuantity);

                                    if (tuple != null)
                                    {
                                        property.SetValuesOfColumn(tuple, balance);
                                    }
                                    else
                                    {
                                        var emptyIndex = Array.FindIndex(ob.Balances,
                                                                         match => match == null);

                                        if (emptyIndex > -1)

                                            ob.Balances[emptyIndex] = balance;
                                    }
                                }
#if DEBUG
                                Status.GetProperites(bal);
#endif
                                return;

                            case Account acc when accounts.Add(acc) is false:

                                var account = accounts.First(o => asset.AccNo.Equals(o.AccNo) &&
                                                                  asset.Date.Equals(o.Date));

                                if (string.IsNullOrEmpty(acc.OrderableCash))
                                {
                                    property.SetValuesOfColumn(account, acc);
                                }
                                else
                                {
                                    if (AccountCollection.TryGetValue(index, out ObservableAccount oa))
                                    {
                                        property.SetValuesOfColumn(oa,
                                                                   new ObservableAccount(account.AccNo,
                                                                                         account.Date,
                                                                                         account.NumberOfPrints,
                                                                                         account.Balance,
                                                                                         account.Asset,
                                                                                         account.PresumeAsset,
                                                                                         account.Deposit,
                                                                                         account.PresumeDeposit,
                                                                                         account.TotalPurchaseAmount,
                                                                                         acc.OrderableCash));

                                        account.Balances ??= oa.NumberOfPrints > 0 ? new Balance[oa.NumberOfPrints] :
                                                                                     Array.Empty<Balance>();
                                    }
                                    account.OrderableCash = acc.OrderableCash;
                                }
#if DEBUG
                                Status.GetProperites(acc);
#endif
                                return;
                        }
                        return;

                    case GroupEventArgs group:
#if DEBUG
                        System.Diagnostics.Debug.WriteLine(group.Name);
#endif
                        groupNames.Push(group.Name);
                        return;
                }
            };
        }
        AccountCollection = new ObservableCollection<ObservableAccount>();
    }
    [RelayCommand]
    async Task SwipeUp(ObservableAccount acc)
    {
        await Shell.Current.GoToAsync(string.Concat(nameof(AssetChartPage),
                                      '?',
                                      nameof(acc.AccNo),
                                      '=',
                                      acc.AccNo,
                                      '&',
                                      nameof(acc.Account),
                                      '=',
                                      acc.Account));
    }
    readonly HashSet<Account> accounts = new();
    readonly Stack<string> groupNames = new();
    readonly IHubService hub;
    readonly IConnectivity connectivity;
    readonly IPropertyService property;
    readonly ILoginService<ObservableAccount> login;
}