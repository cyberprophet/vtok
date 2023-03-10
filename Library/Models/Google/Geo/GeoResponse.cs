using Newtonsoft.Json;

using System.Runtime.Serialization;

namespace ShareInvest.Models.Google;

public struct GeoResponse
{
    [DataMember, JsonProperty("accuracy")]
    public double Accuracy
    {
        get; set;
    }
    [DataMember, JsonProperty("location")]
    public Location Location
    {
        get; set;
    }
    [DataMember, JsonProperty("error")]
    public Error Error
    {
        get; set;
    }
}
public struct Error
{
    [DataMember, JsonProperty("code")]
    public int Code
    {
        get; set;
    }
    [DataMember, JsonProperty("message")]
    public string Message
    {
        get; set;
    }
    [DataMember, JsonProperty("errors")]
    public Errors[] Errors
    {
        get; set;
    }
}
public struct Errors
{
    [DataMember, JsonProperty("domain")]
    public string Domain
    {
        get; set;
    }
    [DataMember, JsonProperty("reason")]
    public string Reason
    {
        get; set;
    }
    [DataMember, JsonProperty("message")]
    public string Message
    {
        get; set;
    }
}
public struct Location
{
    [DataMember, JsonProperty("lat")]
    public double Latitude
    {
        get; set;
    }
    [DataMember, JsonProperty("lng")]
    public double Longitude
    {
        get; set;
    }
}