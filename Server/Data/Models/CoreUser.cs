using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;

namespace ShareInvest.Server.Data.Models;

public class CoreUser : IdentityUser
{
    [StringLength(0x20)]
    public string? LoginProvider
    {
        get; set;
    }
}