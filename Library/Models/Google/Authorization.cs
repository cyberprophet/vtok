using Newtonsoft.Json;

using System.Runtime.Serialization;

namespace ShareInvest.Models.Google;

public class Authorization
{
    [DataMember, JsonProperty("api_authorization")]
    public string? Key
    {
        get; set;
    }
}