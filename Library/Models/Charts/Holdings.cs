namespace ShareInvest.Models.Charts;

public class Holdings
{
    public long Lookup
    {
        get; set;
    }
    public string? AccNo
    {
        get; set;
    }
    public string? Date
    {
        get; set;
    }
    public string? Code
    {
        get; set;
    }
    public string? Name
    {
        get; set;
    }
    public string? Valuation
    {
        get; set;
    }
    public string? Rate
    {
        get; set;
    }
    public Holdings()
    {

    }
    public Holdings(OpenAPI.Response.BalanceOPW00004 bal)
    {
        Rate = bal.Rate ?? string.Empty;
        Valuation = bal.Evaluation ?? string.Empty;
        Name = bal.Name ?? string.Empty;
        Date = bal.Date ?? string.Empty;
        AccNo = bal.AccNo ?? string.Empty;
        Code = bal.Code;
        Lookup = bal.Lookup;
    }
}