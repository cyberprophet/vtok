using CommunityToolkit.Mvvm.ComponentModel;

using System.Globalization;

namespace ShareInvest.Models;

public partial class ObservableStockStatus : ObservableObject
{
    [ObservableProperty]
    int close;

    [ObservableProperty]
    int open;

    [ObservableProperty]
    int high;

    [ObservableProperty]
    int low;

    [ObservableProperty]
    long volume;

    [ObservableProperty]
    DateTime lookup;

    public ObservableStockStatus(string? code,
                                 string? date,
                                 string? close,
                                 string? open,
                                 string? high,
                                 string? low,
                                 string? volume,
                                 string? name)
    {
        Code = code;
        Date = date;
        Name = name;

        lookup = DateTime.TryParseExact(date,
                                        "yyyyMMdd",
                                        CultureInfo.CurrentCulture,
                                        DateTimeStyles.AssumeLocal,
                                        out DateTime dt)
                                        ?
                                        dt :
                                        DateTime.UnixEpoch;

        this.close = Convert.ToInt32(close);
        this.open = Convert.ToInt32(open);
        this.high = Convert.ToInt32(high);
        this.low = Convert.ToInt32(low);
        this.volume = Convert.ToInt64(volume);
    }
    public string? Code
    {
        get;
    }
    public string? Date
    {
        get;
    }
    public string? Name
    {
        get;
    }
}