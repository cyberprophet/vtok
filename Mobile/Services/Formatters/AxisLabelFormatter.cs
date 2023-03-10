using DevExpress.Maui.Charts;

namespace ShareInvest.Services.Formatters;

public class AxisLabelFormatter : IAxisLabelTextFormatter
{
    public string Format(object axisValue) =>

        axisValue switch
        {
            DateTime value =>

                (value.Day is >= 1 and <= 7) ? $"{value:MMM d}" : $"{value.Day}",

            _ => string.Empty
        };
}