using Duende.IdentityServer.EntityFramework.Extensions;
using Duende.IdentityServer.EntityFramework.Options;

using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

using ShareInvest.Models;
using ShareInvest.Models.Dart;
using ShareInvest.Models.OpenAPI;
using ShareInvest.Models.OpenAPI.Response;
using ShareInvest.Server.Data.Models;

namespace ShareInvest.Server.Data;

public class CoreContext : ApiAuthorizationDbContext<CoreUser>
{
    public CoreContext(DbContextOptions options,
                       IOptions<OperationalStoreOptions> store) : base(options, store)
    {
        this.store = store;
    }
    public DbSet<KiwoomUser>? KiwoomUsers
    {
        get; set;
    }
    public DbSet<KiwoomMessage>? KiwoomMessages
    {
        get; set;
    }
    public DbSet<OPTKWFID>? OPTKWFID
    {
        get; set;
    }
    public DbSet<BalanceOPW00004>? Balances
    {
        get; set;
    }
    public DbSet<AccountOPW00004>? Accounts
    {
        get; set;
    }
    public DbSet<BalanceOPW00005>? ClosedBalances
    {
        get; set;
    }
    public DbSet<AccountOPW00005>? ClosedAccounts
    {
        get; set;
    }
    public DbSet<FileVersionInfo>? FileVersions
    {
        get; set;
    }
    public DbSet<IntegrationAccount>? Integrations
    {
        get; set;
    }
    public DbSet<CompanyOverview>? Companies
    {
        get; set;
    }
    public DbSet<Member>? Members
    {
        get; set;
    }
    public DbSet<OPT10081>? KiwoomChart
    {
        get; set;
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder

            .Entity<OPT10081>(build =>
            {
                build.HasKey(propertyName => new
                {
                    propertyName.Code,
                    propertyName.Date
                });
                build.Property(property => property.Start).IsRequired();
                build.Property(property => property.High).IsRequired();
                build.Property(property => property.Low).IsRequired();
                build.Property(property => property.Volume).IsRequired();
                build.Property(property => property.Current).IsRequired();
                build.ToTable(nameof(OPT10081));
            })
            .Entity<Member>(build =>
            {
                build.HasKey(name => name.Key);
                build.ToTable(nameof(Member));
            })
            .Entity<CompanyOverview>(o =>
            {
                o.HasKey(o => o.Code);
                o.ToTable(nameof(Companies));
            })
            .Entity<IntegrationAccount>(o =>
            {
                o.HasKey(o => o.Id);
                o.Property(o => o.SerialNumber).IsRequired();
                o.Property(o => o.AccountNumber).IsRequired();
                o.ToTable(nameof(IntegrationAccount));
            })
            .Entity<FileVersionInfo>(o =>
            {
                o.HasKey(o => new
                {
                    o.App,
                    o.Path,
                    o.FileName
                });
                o.ToTable(nameof(FileVersionInfo));
            })
            .Entity<AccountOPW00005>(o =>
            {
                o.HasKey(o => new
                {
                    o.AccNo,
                    o.Date
                });
                o.ToTable(nameof(AccountOPW00005));
            })
            .Entity<BalanceOPW00005>(o =>
            {
                o.HasKey(o => new
                {
                    o.AccNo,
                    o.Date,
                    o.Code
                });
                o.ToTable(nameof(BalanceOPW00005));
            })
            .Entity<AccountOPW00004>(o =>
            {
                o.HasKey(o => new
                {
                    o.AccNo,
                    o.Date
                });
                o.ToTable(nameof(AccountOPW00004));
            })
            .Entity<BalanceOPW00004>(o =>
            {
                o.HasKey(o => new
                {
                    o.AccNo,
                    o.Date,
                    o.Code
                });
                o.ToTable(nameof(BalanceOPW00004));
            })
            .Entity<OPTKWFID>(o =>
            {
                o.HasKey(o => o.Code);
                o.ToTable(nameof(OPTKWFID));
            })
            .Entity<KiwoomMessage>(o =>
            {
                o.HasKey(o => new
                {
                    o.Key,
                    o.Lookup
                });
                o.ToTable(nameof(KiwoomMessage));
            })
            .Entity<KiwoomUser>(o =>
            {
                o.HasKey(o => new
                {
                    o.Key,
                    o.AccNo
                });
                o.ToTable(nameof(KiwoomUser));
            })
            .ConfigurePersistedGrantContext(store.Value);

        base.OnModelCreating(builder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.ConfigureWarnings(o =>
        {
            o.Log((RelationalEventId.ConnectionOpened,
                   LogLevel.Information),
                  (RelationalEventId.ConnectionClosed,
                   LogLevel.Information));
        })
               .EnableDetailedErrors();
    }
    readonly IOptions<OperationalStoreOptions> store;
}