using DevExpress.Maui.Charts;

using ShareInvest.Models;
using ShareInvest.Services.Providers;
using ShareInvest.ViewModels;

namespace ShareInvest.Pages;

public partial class AssetChartPage : ContentPage
{
    public AssetChartPage(AssetChartViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        if (ViewModel != null)
        {
            await ViewModel.InitializeAsync();

            if (statusChart.Series.Count == 0)
            {
                var adapter = new SeriesDataAdapter
                {
                    DataSource = ViewModel.ChartCollection,
                    ArgumentDataMember = nameof(ObservableAssetStatus.Lookup)
                };
                adapter.ValueDataMembers.Add(new ValueDataMember
                {
                    Type = DevExpress.Maui.Charts.ValueType.Value,
                    Member = nameof(ObservableAssetStatus.PresumeAsset)
                });
                statusChart.Series.Add(new SplineSeries
                {
                    Data = adapter,
                    Style = new LineSeriesStyle
                    {
                        Stroke = Color.FromArgb(AppTheme.Dark == Application.Current?.UserAppTheme ? "#FFE140" : "#CCAC00")
                    },
                    HintOptions = new SeriesCrosshairOptions
                    {
                        PointTextProvider = new PointTextProvider()
                    },
                    DisplayName = nameof(ObservableAssetStatus.PresumeAsset)
                });
            }
        }
        base.OnAppearing();
    }
    protected override async void OnDisappearing()
    {
        if (ViewModel != null)
        {
            await ViewModel.DisposeAsync();
        }
        base.OnDisappearing();
    }
    AssetChartViewModel? ViewModel
    {
        get => BindingContext as AssetChartViewModel;
    }
}