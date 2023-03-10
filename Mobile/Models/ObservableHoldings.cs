using CommunityToolkit.Mvvm.ComponentModel;

using ShareInvest.Models.Charts;

namespace ShareInvest.Models;

public partial class ObservableHoldings : ObservableObject
{
    [ObservableProperty]
    long valuation;

    [ObservableProperty]
    double rate;

    [ObservableProperty]
    DateTime lookup;

    public ObservableHoldings(Holdings holdings)
    {
        AccNo = holdings.AccNo;
        Date = holdings.Date;
        Code = holdings.Code;
        Name = holdings.Name;

        ConvertParameter(holdings.Valuation,
                         holdings.Rate);

        lookup = new DateTime(holdings.Lookup);
    }
    public ObservableHoldings(long lookup,
                              string accNo,
                              string date,
                              string code,
                              string name,
                              string valuation,
                              string rate)
    {
        ConvertParameter(valuation, rate);

        this.lookup = new DateTime(lookup);

        AccNo = accNo;
        Date = date;
        Code = code;
        Name = name;
    }
    public string? AccNo
    {
        get;
    }
    public string? Date
    {
        get;
    }
    public string? Code
    {
        get;
    }
    public string? Name
    {
        get;
    }
    void ConvertParameter(string? valuation,
                          string? rate)
    {
        this.valuation = Convert.ToInt64(valuation);
        this.rate = Convert.ToInt32(rate?[1..]) * 1e-6;
    }
}