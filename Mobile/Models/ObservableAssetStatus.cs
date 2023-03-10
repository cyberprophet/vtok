using CommunityToolkit.Mvvm.ComponentModel;

using ShareInvest.Models.Charts;

namespace ShareInvest.Models;

public partial class ObservableAssetStatus : ObservableObject
{
    [ObservableProperty]
    DateTime lookup;

    [ObservableProperty]
    long presumeDeposit;

    [ObservableProperty]
    long presumeAsset;

    public ObservableAssetStatus(AssetStatus status)
    {
        var holdings = status.Holdings?.Select(s => new ObservableHoldings(s));

        lookup = new DateTime(status.Lookup);

        Date = status.Date;
        AccNo = status.AccNo;
        Holdings = new List<ObservableHoldings>(holdings ?? Array.Empty<ObservableHoldings>());

        ConvertParameter(status.PresumeDeposit, status.PresumeAsset);
    }
    public ObservableAssetStatus(long lookup,
                                 string date,
                                 string accNo,
                                 string presumeDeposit,
                                 string presumeAsset)
    {
        ConvertParameter(presumeDeposit, presumeAsset);

        this.lookup = new DateTime(lookup);

        Date = date;
        AccNo = accNo;
        Holdings = new List<ObservableHoldings>();
    }
    public List<ObservableHoldings>? Holdings
    {
        get;
    }
    public string? Date
    {
        get;
    }
    public string? AccNo
    {
        get;
    }
    void ConvertParameter(string? presumeDeposit,
                          string? presumeAsset)
    {
        this.presumeAsset = Convert.ToInt64(presumeAsset);
        this.presumeDeposit = Convert.ToInt64(presumeDeposit);
    }
}