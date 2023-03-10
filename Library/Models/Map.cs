namespace ShareInvest.Models;

public class Map : Stock
{
    public override string? Code
    {
        get; set;
    }
    public override string? Name
    {
        get; set;
    }
    public override string? Current
    {
        get; set;
    }
    public override string? Rate
    {
        get; set;
    }
    public override string? CompareToPreviousSign
    {
        get; set;
    }
    public override string? CompareToPreviousDay
    {
        get; set;
    }
    public double Longitude
    {
        get; set;
    }
    public double Latitude
    {
        get; set;
    }
}