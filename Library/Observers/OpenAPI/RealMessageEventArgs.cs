namespace ShareInvest.Observers.OpenAPI;

public class RealMessageEventArgs : MessageEventArgs
{
    public string Type
    {
        get;
    }
    public string Key
    {
        get;
    }
    public string Data
    {
        get;
    }
    public RealMessageEventArgs(string type, string key, string data)
    {
        Type = type;
        Key = key;
        Data = data;
    }
    public RealMessageEventArgs(string key, string data)
    {
        Key = key;
        Data = data;
        Type = string.Empty;
    }
}