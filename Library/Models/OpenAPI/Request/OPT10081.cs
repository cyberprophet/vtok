namespace ShareInvest.Models.OpenAPI.Request;

public class OPT10081 : TR
{
    public override string[] Id => new[]
    {
        "종목코드",
        "기준일자",
        "수정주가구분"
    };
    public override string[]? Value
    {
        get; set;
    }
    public override string? RQName
    {
        set
        {

        }
        get => rqName;
    }
    public override string TrCode
    {
        get => nameof(OPT10081);
    }
    public override int PrevNext
    {
        get; set;
    }
    public override string ScreenNo
    {
        get => LookupScreenNo;
    }
    public override string[] Single => new[]
    {
        "종목코드"
    };
    public override string[] Multiple => new[]
    {
        "종목코드",
        "현재가",
        "거래량",
        "거래대금",
        "일자",
        "시가",
        "고가",
        "저가",
        "수정주가구분",
        "수정비율",
        "대업종구분",
        "소업종구분",
        "종목정보",
        "수정주가이벤트",
        "전일종가"
    };
    const string rqName = "주식일봉차트조회요청";
}