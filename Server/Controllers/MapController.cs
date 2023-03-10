using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using ShareInvest.Models;
using ShareInvest.Server.Data;

namespace ShareInvest.Server.Controllers;

[ApiController,
 Produces("application/json"),
 Route("[controller]"),
 ProducesResponseType(StatusCodes.Status200OK),
 ProducesResponseType(StatusCodes.Status204NoContent)]
public class MapController : ControllerBase
{
    [ApiExplorerSettings(GroupName = "account"),
     HttpGet("{id}")]
    public async Task<IActionResult> GetStockThatInvestedTheHighestAsync([FromRoute] string id)
    {
        if (context.Integrations != null &&
            context.Balances != null &&
            context.Companies != null)
        {
            var accounts = (from user in context.UserLogins.AsNoTracking()
                            where id.Equals(user.UserId)
                            join i in context.Integrations.AsNoTracking()
                            on user.ProviderKey equals i.ProviderKey
                            select i.AccountNumber)
                            .ToArray();

            var stock = (await (from bal in context.Balances.AsNoTracking()
                                join company in context.Companies.AsNoTracking()
                                on bal.Code equals company.Code
                                orderby bal.Date descending,
                                        bal.Evaluation descending
                                select new
                                {
                                    bal.Code,
                                    bal.Name,
                                    company.Latitude,
                                    company.Longitude,
                                    bal.Date,
                                    bal.Evaluation,
                                    bal.AccNo
                                })
                                .ToArrayAsync())
                                .FirstOrDefault(p => Array.Exists(accounts,
                                                                  m => m.Equals(p.AccNo)));
            if (stock != null)
            {
                logger.LogInformation("{ }\n{ }",
                                      id,
                                      JsonConvert.SerializeObject(stock,
                                                                  Formatting.Indented));
                return Ok(stock);
            }
            else
            {
                logger.LogWarning("{ } could not return a satisfactory result.",
                                  id);

                return Ok(new Models.Google.Location
                {
                    Latitude = 37.4031971,
                    Longitude = 127.1059259
                });
            }
        }
        return NoContent();
    }
    [ApiExplorerSettings(GroupName = "stock"),
     HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        if (context.OPTKWFID != null &&
            context.Companies != null)
        {
            var dao = context.OPTKWFID.AsNoTracking();

            if (await dao.MaxAsync(o => o.Date) is string today)
            {
                return Ok(from o in dao
                          where today.Equals(o.Date)
                          join c in context.Companies.AsNoTracking()
                          on o.Code equals c.Code
                          where c.Longitude != 0 && c.Latitude != 0
                          select new Map
                          {
                              Code = o.Code,
                              CompareToPreviousDay = o.CompareToPreviousDay,
                              CompareToPreviousSign = o.CompareToPreviousSign,
                              Current = o.Current,
                              Name = o.Name,
                              Rate = o.Rate,
                              Latitude = c.Latitude,
                              Longitude = c.Longitude,
                              State = o.State,
                              TransactionAmount = o.TransactionAmount,
                              Volume = o.Volume
                          });
            }
            logger.LogError("problem invoking to display corporate information on map.");
        }
        return NoContent();
    }
    public MapController(ILogger<MapController> logger,
                         CoreContext context)
    {
        this.context = context;
        this.logger = logger;
    }
    readonly ILogger<MapController> logger;
    readonly CoreContext context;
}