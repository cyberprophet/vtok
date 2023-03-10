using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Mappers;
using ShareInvest.Models.OpenAPI.Response;
using ShareInvest.Server.Data;

namespace ShareInvest.Server.Controllers.OpenAPI;

public class OPT10081Controller : KiwoomController
{
    [ApiExplorerSettings(GroupName = "stock"),
     HttpGet("[action]")]
    public async Task<IActionResult> DailyChartAsync([FromQuery] string? date,
                                                     [FromQuery] string code,
                                                     [FromQuery] int period)
    {
        if (context.KiwoomChart != null &&
            context.OPTKWFID != null)

            try
            {
                var name = (await context.OPTKWFID.AsNoTracking()
                                                  .SingleAsync(p => code.Equals(p.Code))).Name;

                if (string.IsNullOrEmpty(date))

                    return Ok((from kc in context.KiwoomChart.AsNoTracking()
                               where code.Equals(kc.Code)
                               orderby kc.Date descending
                               select new Models.Chart
                               {
                                   Code = kc.Code,
                                   Current = kc.Current,
                                   Date = kc.Date,
                                   High = kc.High,
                                   Low = kc.Low,
                                   Start = kc.Start,
                                   Volume = kc.Volume,
                                   Name = name
                               })
                               .Take(period));

                return Ok((from kc in context.KiwoomChart.AsNoTracking()
                           where code.Equals(kc.Code) && kc.Date!.CompareTo(date) < 0
                           orderby kc.Date descending
                           select new Models.Chart
                           {
                               Code = kc.Code,
                               Current = kc.Current,
                               Date = kc.Date,
                               High = kc.High,
                               Low = kc.Low,
                               Start = kc.Start,
                               Volume = kc.Volume,
                               Name = name
                           })
                           .Take(period));
            }
            catch (Exception ex)
            {
                logger.LogError("exception occurred while querying { }.",
                                code);

                return BadRequest(ex.Message);
            }
        return NoContent();
    }
    [ApiExplorerSettings(GroupName = "stock"),
     HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        if (context.KiwoomChart != null &&
            context.OPTKWFID != null)
        {
            var now = DateTime.Now.ToString("yyyyMMdd");

            var queryable = context.KiwoomChart.AsNoTracking()
                                               .Where(p => now.Equals(p.Date))
                                               .Select(p => p.Code);

            foreach (var code in context.OPTKWFID.AsNoTracking()
                                                 .Where(p => now.Equals(p.Date))
                                                 .Select(s => s.Code)
                                                 .ToArray()
                                                 .Except(queryable)
                                                 .OrderBy(key => Guid.NewGuid())
                                                 .Take(1))
            {
                logger.LogInformation("stocks to inquire is { }.", code);

                return Ok(code);
            }
            logger.LogWarning("{ }", await queryable.CountAsync());
        }
        return NoContent();
    }
    [ApiExplorerSettings(GroupName = "stock"),
     HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] OPT10081 param)
    {
        if (context.KiwoomChart is not null &&
            string.IsNullOrEmpty(param.Date) is false &&
            string.IsNullOrEmpty(param.Code) is false)
        {
            var tuple = await context.KiwoomChart.FindAsync(param.Code,
                                                            param.Date);

            if (tuple != null)
            {
                service.SetValuesOfColumn(tuple, param);
            }
            else
                context.KiwoomChart.Add(param);

            return Ok(context.SaveChanges());
        }
        logger.LogError("unaffordable amount came in { }.",
                        param.Name);

        return NoContent();
    }
    public OPT10081Controller(CoreContext context,
                              IPropertyService service,
                              ILogger<OPT10081Controller> logger)
    {
        this.context = context;
        this.service = service;
        this.logger = logger;
    }
    readonly IPropertyService service;
    readonly ILogger<OPT10081Controller> logger;
    readonly CoreContext context;
}