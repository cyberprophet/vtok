using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models.OpenAPI;

public class KiwoomMessage : Message
{
    [Key, Column(Order = 1)]
    public override long Lookup
    {
        get; set;
    }
    [Key, Column(Order = 2), StringLength(0x25)]
    public override string? Key
    {
        get; set;
    }
    [Required]
    public string? Title
    {
        get; set;
    }
    [Required]
    public string? Code
    {
        get; set;
    }
    [Required, StringLength(4)]
    public string? Screen
    {
        get; set;
    }
}