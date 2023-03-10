using Newtonsoft.Json;

using System.Runtime.Serialization;

namespace ShareInvest.Models.Google;

public struct GeoRequest
{
    [JsonProperty("considerIp"), DataMember]
    public bool ConsiderIp
    {
        get; set;
    }
    [JsonProperty("wifiAccessPoints"), DataMember]
    public WifiAccessPoint[] WifiAccessPoints
    {
        get; set;
    }
}
public struct WifiAccessPoint
{
    [JsonProperty("macAddress"), DataMember]
    public string MacAddress
    {
        get; set;
    }
    [JsonProperty("signalStrength"), DataMember]
    public double SignalStrength
    {
        get; set;
    }
    [JsonProperty("signalToNoiseRatio"), DataMember]
    public double SignalToNoiseRatio
    {
        get; set;
    }
}