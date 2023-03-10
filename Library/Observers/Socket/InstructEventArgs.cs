using ShareInvest.Models;
using ShareInvest.Models.Charts;
using ShareInvest.Models.OpenAPI.Response;

namespace ShareInvest.Observers.Socket;

public class InstructEventArgs : MessageEventArgs
{
    public object Convey
    {
        get;
    }
    public string AccNo
    {
        get;
    }
    public string Date
    {
        get;
    }
    public InstructEventArgs(string account)
    {
        AccNo = account;
        Convey = account;
        Date = string.Empty;
    }
    public InstructEventArgs(Balance bal)
    {
        Convey = bal;
        Date = bal.Date ?? string.Empty;
        AccNo = bal.AccNo ?? string.Empty;
    }
    public InstructEventArgs(Account asset)
    {
        Convey = asset;
        AccNo = asset.AccNo ?? string.Empty;
        Date = asset.Date ?? string.Empty;
    }
    public InstructEventArgs(AccountOPW00004 opw)
    {
        Convey = opw;
        AccNo = opw.AccNo ?? string.Empty;
        Date = opw.Date ?? string.Empty;
    }
    public InstructEventArgs(AccountOPW00005 opw)
    {
        Convey = opw;
        AccNo = opw.AccNo ?? string.Empty;
        Date = opw.Date ?? string.Empty;
    }
    public InstructEventArgs(BalanceOPW00004 opw)
    {
        Convey = opw;
        AccNo = opw.AccNo ?? string.Empty;
        Date = opw.Date ?? string.Empty;
    }
    public InstructEventArgs(BalanceOPW00005 opw)
    {
        Convey = opw;
        AccNo = opw.AccNo ?? string.Empty;
        Date = opw.Date ?? string.Empty;
    }
    public InstructEventArgs(AssetStatus status)
    {
        Convey = status;
        AccNo = status.AccNo ?? string.Empty;
        Date = status.Date ?? string.Empty;
    }
}