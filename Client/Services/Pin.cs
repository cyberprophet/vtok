using ShareInvest.Client.Properties;
using ShareInvest.Models;

namespace ShareInvest.Client.Services;

class Pin
{
    internal Stack<object> MakePins(IEnumerable<Member> members)
    {
        foreach (var m in members)

            args.Push(new
            {
                position = new
                {
                    lat = m.Latitude,
                    lng = m.Longitude
                },
                png = Status.MakePinImage(Resources.AI),
                name = m.Name,
                code = m.Key
            });
        return args;
    }
    internal Stack<object> MakePins(Dictionary<string, Map> stocks)
    {
        foreach (var kv in stocks)

            if (int.TryParse(kv.Value.CompareToPreviousDay?[0] is '-' ? kv.Value.CompareToPreviousDay[1..] :
                                                                        kv.Value.CompareToPreviousDay,
                             out int day) &&

                int.TryParse(kv.Value.Current?[0] is '-' ? kv.Value.Current[1..] :
                                                           kv.Value.Current,
                             out int current) &&

                double.TryParse(kv.Value.Rate?[0] is '-' ? kv.Value.Rate[1..] :
                                                           kv.Value.Rate,
                                out double rate))
            {
                args.Push(new
                {
                    position = new
                    {
                        lat = kv.Value.Latitude,
                        lng = kv.Value.Longitude
                    },
                    contents = new
                    {
                        code = kv.Key,
                        sign = kv.Value.CompareToPreviousSign,
                        html = kv.Value.CompareToPreviousSign switch
                        {
                            "2" => string.Concat("<span class=\"oi oi-caret-top oi-padding\">",
                                                 "</span>",
                                                 day),

                            "5" => string.Concat("<span class=\"oi oi-caret-bottom oi-padding\">",
                                                 "</span>",
                                                 day),

                            "1" => string.Concat("<span class=\"oi oi-arrow-thick-top oi-padding\">",
                                                 "</span>",
                                                 day),

                            "4" => string.Concat("<span class=\"oi oi-arrow-thick-bottom oi-padding\">",
                                                 "</span>",
                                                 day),
                            _ => string.Empty
                        },
                        after = current.ToString("N0"),
                        before = rate > 0 ? (rate * 1e-2).ToString("P2") : string.Empty
                    },
                    png = Status.MakePinImage(kv.Value.CompareToPreviousSign switch
                    {
                        "2" or "1" => "red",
                        "5" or "4" => "blue",
                        _ => "black"
                    }),
                    name = kv.Value.Name,
                    code = kv.Key
                });
            }
        return args;
    }
    readonly Stack<object> args = new();
}