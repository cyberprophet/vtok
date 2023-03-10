using ShareInvest.Pages;
using ShareInvest.Shells;

namespace ShareInvest;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

#if WINDOWS||MACCATALYST

#else
        MainPage = new MobileShell();

        Routing.RegisterRoute(nameof(AssetChartPage), typeof(AssetChartPage));
        Routing.RegisterRoute(nameof(StockChartPage), typeof(StockChartPage));
#endif
    }
}