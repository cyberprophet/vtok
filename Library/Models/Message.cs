namespace ShareInvest.Models;

public abstract class Message
{
    public abstract long Lookup
    {
        get; set;
    }
    public abstract string? Key
    {
        get; set;
    }
}