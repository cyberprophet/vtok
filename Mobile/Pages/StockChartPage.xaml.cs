using ShareInvest.ViewModels;

namespace ShareInvest.Pages;

public partial class StockChartPage : ContentPage
{
    public StockChartPage(StockChartViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
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
    async void OnLoaded(object sender, EventArgs _)
    {
        if (ViewModel != null)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(sender.GetType().Name);
#endif
            await ViewModel.LoadChartAsync();
        }
    }
    StockChartViewModel? ViewModel
    {
        get => BindingContext as StockChartViewModel;
    }
}