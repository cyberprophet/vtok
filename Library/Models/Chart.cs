using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models;

public class Chart
{
    [NotMapped]
    public string? Name
    {
        get; set;
    }
    public virtual string? Code
    {
        get; set;
    }
    public virtual string? Current
    {
        get; set;
    }
    public virtual string? Volume
    {
        get; set;
    }
    public virtual string? Date
    {
        get; set;
    }
    public virtual string? Start
    {
        get; set;
    }
    public virtual string? High
    {
        get; set;
    }
    public virtual string? Low
    {
        get; set;
    }
}