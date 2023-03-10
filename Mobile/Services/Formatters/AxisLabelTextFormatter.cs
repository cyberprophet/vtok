using DevExpress.Maui.Charts;

using System.Globalization;

namespace ShareInvest.Services.Formatters;

public class AxisLabelTextFormatter : IAxisLabelTextFormatter
{
    public string Format(object value) =>

        value switch
        {
            long number => new Func<string>(() =>
            {
                if (number % HUNDREDMILLION == 0)
                    return string.Concat(number / HUNDREDMILLION, '억');

                if (number % TENMILLION == 0)
                    return string.Concat(number / TENMILLION, "천만");

                if (number % MILLION == 0)
                    return string.Concat(number / MILLION, "백만");

                if (number % HUNDREDTHOUSAND == 0)
                    return string.Concat(number / HUNDREDTHOUSAND, "십만");

                if (number % TENTHOUSAND == 0)
                    return string.Concat(number / TENTHOUSAND, '만');

                if (number % THOUSAND == 0)
                    return string.Concat(number / THOUSAND, '천');

                return Convert.ToString(number);
            })(),

            double floatingNumber => new Func<string>(() =>
            {
                if (floatingNumber == 0)
                    return string.Empty;

                if (floatingNumber % HUNDREDMILLION == 0 ||
                    floatingNumber % TENMILLION == 0)
                    return string.Concat(floatingNumber / HUNDREDMILLION, '억');

                if (floatingNumber % MILLION == 0)
                    return string.Empty;

                if (floatingNumber % HUNDREDTHOUSAND == 0 ||
                    floatingNumber % TENTHOUSAND == 0)
                    return string.Concat(floatingNumber / TENTHOUSAND, '만');

                if (floatingNumber % THOUSAND == 0)
                    return string.Empty;

                return floatingNumber.ToString("C0");
            })(),

            DateTime axisValue => axisValue.ToString("MMM d yyyy",
                                                     CultureInfo.GetCultureInfo("en-US")),

            _ => string.Empty
        };
    const int HUNDREDMILLION = 100_000_000;
    const int TENMILLION = 10_000_000;
    const int MILLION = 1_000_000;
    const int HUNDREDTHOUSAND = 100_000;
    const int TENTHOUSAND = 10_000;
    const int THOUSAND = 1_000;
}