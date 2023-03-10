namespace ShareInvest.Mappers.Kiwoom;

public enum EnumKiwoomRevise
{
    유상증자 = 1,
    무상증자 = 2,
    배당락 = 4,
    액면분할 = 8,
    액면병합 = 16,
    기업합병 = 32,
    감자 = 64,
    권리락 = 256
}
public enum EnumChartDuration
{
    DailyChart = 'D'
}
public enum EnumMarketOperation
{
    장시작전 = 0,
    장마감전_동시호가 = 2,
    장시작 = 3,
    장종료_예상지수종료 = 4,
    장마감 = 8,
    장종료_시간외종료 = 9,
    시간외_종가매매_시작 = 'a',
    시간외_종가매매_종료 = 'b',
    시간외_단일가_매매시작 = 'c',
    시간외_단일가_매매종료 = 'd',
    선옵_장마감전_동시호가_시작 = 's',
    선옵_장마감전_동시호가_종료 = 'e'
}
public static class MarketOperation
{
    public static EnumMarketOperation Get(string? arg)
    {
        if (arg?.Length == 1)
        {
            var index = char.IsDigit(arg[0]) ? Convert.ToInt32(arg) :
                                               Convert.ToChar(arg);

            if (Enum.IsDefined(typeof(EnumMarketOperation), index))
                return (EnumMarketOperation)index;
        }
        return EnumMarketOperation.장종료_시간외종료;
    }
}