using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models;

public class Member
{
    public string? Key
    {
        get; set;
    }
    public double Accuracy
    {
        get; set;
    }
    public double Latitude
    {
        get; set;
    }
    public double Longitude
    {
        get; set;
    }
    [NotMapped]
    public string? Name
    {
        get; set;
    }
}