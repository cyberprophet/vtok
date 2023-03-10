namespace ShareInvest.Models;

public class Account
{
    public virtual Balance[]? Balances
    {
        get; set;
    }
    public string? NumberOfPrints
    {
        get; set;
    }
    public string? Balance
    {
        get; set;
    }
    public string? Asset
    {
        get; set;
    }
    public string? PresumeAsset
    {
        get; set;
    }
    public string? Deposit
    {
        get; set;
    }
    public string? PresumeDeposit
    {
        get; set;
    }
    public string? TotalPurchaseAmount
    {
        get; set;
    }
    public string? OrderableCash
    {
        get; set;
    }
    public string? Date
    {
        get; set;
    }
    public string? AccNo
    {
        get; set;
    }
}