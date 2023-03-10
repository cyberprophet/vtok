namespace ShareInvest.Models;

public abstract class AccountBook
{
    public abstract string? AccNo
    {
        get; set;
    }
    public abstract string? Date
    {
        get; set;
    }
    public abstract long Lookup
    {
        get; set;
    }
}