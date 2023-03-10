using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ShareInvest.Models.OpenAPI.Response;

public class OPT10081 : Chart
{
    [DataMember, JsonProperty("종목코드"), Key, StringLength(8)]
    public override string? Code
    {
        get; set;
    }
    [DataMember, JsonProperty("현재가"), StringLength(0x10), Required]
    public override string? Current
    {
        get; set;
    }
    [DataMember, JsonProperty("거래량"), StringLength(0x10), Required]
    public override string? Volume
    {
        get; set;
    }
    [DataMember, JsonProperty("거래대금"), StringLength(0x10)]
    public string? Amount
    {
        get; set;
    }
    [DataMember, JsonProperty("일자"), Key, StringLength(8)]
    public override string? Date
    {
        get; set;
    }
    [DataMember, JsonProperty("시가"), StringLength(0x10), Required]
    public override string? Start
    {
        get; set;
    }
    [DataMember, JsonProperty("고가"), StringLength(0x10), Required]
    public override string? High
    {
        get; set;
    }
    [DataMember, JsonProperty("저가"), StringLength(0x10), Required]
    public override string? Low
    {
        get; set;
    }
    [DataMember, JsonProperty("수정주가구분"), StringLength(0x10)]
    public string? Revise
    {
        get; set;
    }
    [DataMember, JsonProperty("수정비율"), StringLength(0x10)]
    public string? ReviseRate
    {
        get; set;
    }
    [DataMember, JsonProperty("대업종구분"), StringLength(0x10)]
    public string? MainCategory
    {
        get; set;
    }
    [DataMember, JsonProperty("소업종구분"), StringLength(0x10)]
    public string? SubCategory
    {
        get; set;
    }
    [DataMember, JsonProperty("종목정보"), StringLength(0x10)]
    public string? StockInfo
    {
        set; get;
    }
    [DataMember, JsonProperty("수정주가이벤트"), StringLength(0x10)]
    public string? ReviseEvent
    {
        get; set;
    }
    [DataMember, JsonProperty("전일종가"), StringLength(0x10)]
    public string? Close
    {
        get; set;
    }
}