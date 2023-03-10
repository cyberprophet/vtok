using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ShareInvest.Models.Dart;

public class CompanyOverview : DartCode
{
    public DateTime Date
    {
        get; set;
    }
    public double Latitude
    {
        get; set;
    }
    public double Longitude
    {
        get; set;
    }
    [DataMember, JsonProperty("status")]
    public string? Status
    {
        get; set;
    }
    [DataMember, JsonProperty("message")]
    public string? Message
    {
        get; set;
    }
    [DataMember, JsonProperty("corp_code"), StringLength(8)]
    public override string? CorpCode
    {
        get; set;
    }
    [DataMember, JsonProperty("corp_name"), StringLength(0x40)]
    public override string? CorpName
    {
        get; set;
    }
    [DataMember, JsonProperty("modify_date"), StringLength(8)]
    public override string? ModifyDate
    {
        get; set;
    }
    [DataMember, JsonProperty("corp_name_eng")]
    public string? CorpEngName
    {
        get; set;
    }
    [DataMember, JsonProperty("stock_name")]
    public string? Name
    {
        get; set;
    }
    [DataMember, JsonProperty("stock_code"), Key, StringLength(6)]
    public override string? Code
    {
        get; set;
    }
    [DataMember, JsonProperty("ceo_nm")]
    public string? CEO
    {
        get; set;
    }
    [DataMember, JsonProperty("corp_cls")]
    public string? Classification
    {
        get; set;
    }
    [DataMember, JsonProperty("jurir_no")]
    public string? LegalRegistrationNumber
    {
        get; set;
    }
    [DataMember, JsonProperty("bizr_no")]
    public string? CorporateRegistrationNumber
    {
        get; set;
    }
    [DataMember, JsonProperty("adres")]
    public string? Address
    {
        get; set;
    }
    [DataMember, JsonProperty("hm_url")]
    public string? Url
    {
        get; set;
    }
    [DataMember, JsonProperty("ir_url")]
    public string? IR
    {
        get; set;
    }
    [DataMember, JsonProperty("phn_no")]
    public string? Phone
    {
        get; set;
    }
    [DataMember, JsonProperty("fax_no")]
    public string? Fax
    {
        get; set;
    }
    [DataMember, JsonProperty("induty_code")]
    public string? IndutyCode
    {
        get; set;
    }
    [DataMember, JsonProperty("est_dt")]
    public string? FoundingDate
    {
        get; set;
    }
    [DataMember, JsonProperty("acc_mt")]
    public string? SettlementMonth
    {
        get; set;
    }
}