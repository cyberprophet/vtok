using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models.OpenAPI;

public class KiwoomUser : User
{
    [StringLength(0x10), Key]
    public override string? AccNo
    {
        get; set;
    }
    [Key]
    public override string? Key
    {
        get; set;
    }
    public string? Name
    {
        get; set;
    }
    [Required]
    public string? Id
    {
        get; set;
    }
    [NotMapped]
    public string[]? Accounts
    {
        get; set;
    }
    public int NumberOfAccounts
    {
        get; set;
    }
    public bool IsNotMock
    {
        get; set;
    }
    public override bool IsAdministrator
    {
        get; set;
    }
}