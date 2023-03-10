using Newtonsoft.Json;

using ShareInvest.Services;
using ShareInvest.ViewModels;

using System.Diagnostics;

namespace ShareInvest.Pages;

public partial class AccountsPage : ContentPage
{
    public AccountsPage(AccountsViewModel vm)
    {
        InitializeComponent();

        PropertyService.SetStatusBarColor();

        BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        if (ViewModel is not null)
        {
            await ViewModel.InitializeAsync();
        }
        base.OnAppearing();
    }
    protected override async void OnDisappearing()
    {
        if (ViewModel is not null)
        {
            await ViewModel.DisposeAsync();
        }
        base.OnDisappearing();
    }
    void OnSwiped(object sender, SwipedEventArgs e)
    {
#if DEBUG
        Debug.WriteLine(JsonConvert.SerializeObject(new
        {
            sender.GetType().Name,
            e.Direction,
            e.Parameter
        },
        Formatting.Indented));
#endif
    }
    AccountsViewModel? ViewModel
    {
        get => BindingContext as AccountsViewModel;
    }
}