using Newtonsoft.Json;

using ShareInvest.Services;
using ShareInvest.ViewModels;

namespace ShareInvest.Pages;

public partial class StocksPage : ContentPage
{
    public StocksPage(StocksViewModel vm)
    {
        InitializeComponent();

        PropertyService.SetStatusBarColor();

        BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        if (ViewModel != null)
        {
            await ViewModel.InitializeAsync();
#if DEBUG
            System.Diagnostics.Debug.WriteLine(nameof(OnAppearing));
#endif
        }
        base.OnAppearing();
    }
    protected override async void OnDisappearing()
    {
        if (ViewModel != null)
        {
            await ViewModel.DisposeAsync();
#if DEBUG
            System.Diagnostics.Debug.WriteLine(nameof(OnDisappearing));
#endif
        }
        base.OnDisappearing();
    }
    protected override bool OnBackButtonPressed()
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine(nameof(OnBackButtonPressed));
#endif
        return base.OnBackButtonPressed();
    }
    async void OnReceiveRemainingItems(object sender, EventArgs _)
    {
        if (ViewModel != null)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(sender.GetType().Name);
#endif
            await ViewModel.LoadStocksAsync();
        }
    }
    void OnSwiped(object sender, SwipedEventArgs e)
    {
#if DEBUG
        var json = JsonConvert.SerializeObject(new
        {
            sender.GetType().Name,
            e.Direction,
            e.Parameter
        },
        Formatting.Indented);

        System.Diagnostics.Debug.WriteLine(json);
#endif
    }
    StocksViewModel? ViewModel
    {
        get => BindingContext as StocksViewModel;
    }
}