namespace ShareInvest.Models.Charts;

public class AssetStatus
{
    public virtual ICollection<Holdings>? Holdings
    {
        get; set;
    }
    public long Lookup
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
    public string? PresumeDeposit
    {
        get; set;
    }
    public string? PresumeAsset
    {
        get; set;
    }
}