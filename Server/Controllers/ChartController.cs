using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Models.Charts;
using ShareInvest.Server.Data;

namespace ShareInvest.Server.Controllers;

[Route("[controller]"),
 Produces("application/json"),
 ProducesResponseType(StatusCodes.Status200OK),
 ProducesResponseType(StatusCodes.Status204NoContent),
 ApiController]
public class ChartController : ControllerBase
{
    [ApiExplorerSettings(GroupName = "account"),
     Authorize,
     HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] string accNo)
    {
        if (context.Accounts != null &&
            context.Balances != null)
        {
            var lookup = long.MaxValue;

            var list = new List<AssetStatus>();

            do
            {
                var queryable = (from a in context.Accounts.AsNoTracking()
                                 where accNo.Equals(a.AccNo) && lookup > a.Lookup
                                 orderby a.Lookup descending
                                 select new AssetStatus
                                 {
                                     PresumeDeposit = a.PresumeDeposit,
                                     PresumeAsset = a.PresumeAsset,
                                     Lookup = a.Lookup,
                                     AccNo = a.AccNo,
                                     Date = a.Date,
                                     Holdings = (from b in context.Balances.AsNoTracking()

                                                 where string.IsNullOrEmpty(a.AccNo) == false &&
                                                       a.AccNo.Equals(b.AccNo) &&
                                                       string.IsNullOrEmpty(a.Date) == false &&
                                                       a.Date.Equals(b.Date)

                                                 select new Holdings(b)).ToArray()
                                 })
                                 .Take(0x40);

                list.AddRange(queryable);

                if (await queryable.CountAsync() < 0x40)
                {
                    logger.LogInformation("the number of lists is { }.", list.Count);

                    break;
                }
                lookup = queryable.Min(selector => selector.Lookup);
            }
            while (lookup > 0);

            if (list.Count > 0)
                return Ok(list);
        }
        return NoContent();
    }
    public ChartController(CoreContext context,
                           ILogger<ChartController> logger)
    {
        this.context = context;
        this.logger = logger;
    }
    readonly ILogger<ChartController> logger;
    readonly CoreContext context;
}