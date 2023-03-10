using DevExpress.Maui.Charts;

using ShareInvest.Models;

namespace ShareInvest.Services.Providers;

public class PointTextProvider : IPointTextProvider
{
    public string GetText(SeriesPointData seriesPointData)
    {
        foreach (var point in seriesPointData.PointKeyObjects)
        {
            if (point is not DataSourceKey source)
                continue;

            return source.DataObject switch
            {
                ObservableAssetStatus asset =>

                    string.Concat(nameof(asset.PresumeAsset),
                                  ' ',
                                  asset.PresumeAsset.ToString("C0")),

                _ => string.Empty
            };
        }
        return string.Empty;
    }
}