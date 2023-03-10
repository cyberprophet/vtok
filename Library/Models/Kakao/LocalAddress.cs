using Newtonsoft.Json;

using System.Runtime.Serialization;

namespace ShareInvest.Models.Kakao;

public struct LocalAddress
{
    [DataMember, JsonProperty("meta")]
    public Meta Meta
    {
        get; set;
    }
    [DataMember, JsonProperty("documents")]
    public Documents[] Document
    {
        get; set;
    }
}