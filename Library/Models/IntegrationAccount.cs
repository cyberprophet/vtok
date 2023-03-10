using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations;

namespace ShareInvest.Models;

public class IntegrationAccount
{
    [Key]
    public int Id
    {
        get; set;
    }
    [StringLength(0x80), Required]
    public string? ProviderKey
    {
        get; set;
    }
    [StringLength(0x80), Required]
    public string? LoginProvider
    {
        get; set;
    }
    [StringLength(0x10), Required]
    public string? AccountNumber
    {
        get; set;
    }
    [StringLength(0x40), Required]
    public string? SerialNumber
    {
        get; set;
    }
}