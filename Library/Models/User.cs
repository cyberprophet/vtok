namespace ShareInvest.Models;

public abstract class User
{
    public abstract string? Key
    {
        get; set;
    }
    public abstract string? AccNo
    {
        get; set;
    }
    public abstract bool IsAdministrator
    {
        get; set;
    }
}