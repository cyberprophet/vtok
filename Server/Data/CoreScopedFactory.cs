using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Server.Data;

public class CoreScopedFactory : IDbContextFactory<CoreContext>
{
    public CoreContext CreateDbContext()
    {
        var context = this.context.CreateDbContext();

        return context;
    }
    public CoreScopedFactory(IDbContextFactory<CoreContext> context)
    {
        this.context = context;
    }
    readonly IDbContextFactory<CoreContext> context;
}